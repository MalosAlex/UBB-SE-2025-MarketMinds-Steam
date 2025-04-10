using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Data
{
    public sealed partial class DataLink : IDataLink
    {
        private static readonly Lazy<DataLink> DataLinkInstance = new(() => new DataLink());

        // Constants for configuration keys and error messages
        private const string AppSettingsFileName = "appsettings.json";
        private const bool OptionalAppSettings = false;
        private const bool ReloadOnChange = true;

        private const string LocalDataSourceKey = "LocalDataSource";
        private const string InitialCatalogKey = "InitialCatalog";
        private const string UserIdKey = "UserId";
        private const string PasswordKey = "Password";

        private const string MissingConfigErrorMessage = "Database connection settings are missing in appsettings.json";
        private const string ConnectionInitErrorMessage = "Failed to initialize database connection.";
        private const string ConnectionFailedErrorMessage = "Failed to establish database connection. Please check your connection settings.";

        private const string ExecuteScalarErrorMessage = "Database error during ExecuteScalar operation: ";
        private const string ExecuteScalarCastErrorMessage = "Error during ExecuteScalar operation: ";
        private const string ExecuteScalarUnexpectedErrorMessage = "Unexpected error during ExecuteScalar operation: ";

        private const string ExecuteReaderErrorMessage = "Database error during ExecuteReader operation: ";
        private const string ExecuteReaderUnexpectedErrorMessage = "Error during ExecuteReader operation: ";

        private const string ExecuteNonQueryErrorMessage = "Database error during ExecuteNonQuery operation: ";
        private const string ExecuteNonQueryUnexpectedErrorMessage = "Error during ExecuteNonQuery operation: ";

        private const string ExecuteReaderAsyncErrorMessage = "Database error during ExecuteReaderAsync operation: ";
        private const string ExecuteReaderAsyncUnexpectedErrorMessage = "Error during ExecuteReaderAsync operation: ";

        private const string ExecuteNonQueryAsyncErrorMessage = "Database error during ExecuteNonQueryAsync operation: ";
        private const string ExecuteNonQueryAsyncUnexpectedErrorMessage = "Error during ExecuteNonQueryAsync operation: ";

        private const bool UseIntegratedSecurity = true;
        private const bool TrustServerCertificate = true;

        private readonly string connectionString;
        private bool isConnectionDisposed;

        private DataLink()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(AppSettingsFileName, OptionalAppSettings, ReloadOnChange)
                    .Build();

                string? localDataSource = config[LocalDataSourceKey];
                string? initialCatalog = config[InitialCatalogKey];
                string? userId = config[UserIdKey];
                string? password = config[PasswordKey];

                if (string.IsNullOrWhiteSpace(localDataSource) || string.IsNullOrWhiteSpace(initialCatalog))
                {
                    throw new ConfigurationErrorsException(MissingConfigErrorMessage);
                }

                // Integrated Security connection string
                connectionString = $"Data Source={localDataSource};Initial Catalog={initialCatalog};Integrated Security={UseIntegratedSecurity};TrustServerCertificate={TrustServerCertificate};";

                // Test the connection immediately
                using var testConnection = new SqlConnection(connectionString);
                testConnection.Open();
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException(ConnectionFailedErrorMessage, ex);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(ConnectionInitErrorMessage, ex);
            }
        }

        public static DataLink Instance => DataLinkInstance.Value;

        private SqlConnection CreateConnection()
        {
            if (isConnectionDisposed)
            {
                throw new ObjectDisposedException(nameof(DataLink));
            }

            return new SqlConnection(connectionString);
        }

        public T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                connection.Open();
                var result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return default;
                }

                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(result, Nullable.GetUnderlyingType(typeof(T))!);
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ExecuteScalarErrorMessage + ex.Message, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new DatabaseOperationException(ExecuteScalarCastErrorMessage + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ExecuteScalarUnexpectedErrorMessage + ex.Message, ex);
            }
        }

        public DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                using var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ExecuteReaderErrorMessage + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ExecuteReaderUnexpectedErrorMessage + ex.Message, ex);
            }
        }

        public int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();

                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                return command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ExecuteNonQueryErrorMessage + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ExecuteNonQueryUnexpectedErrorMessage + ex.Message, ex);
            }
        }

        public async Task<DataTable> ExecuteReaderAsync(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ExecuteReaderAsyncErrorMessage + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ExecuteReaderAsyncUnexpectedErrorMessage + ex.Message, ex);
            }
        }

        public async Task ExecuteNonQueryAsync(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseOperationException(ExecuteNonQueryAsyncErrorMessage + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException(ExecuteNonQueryAsyncUnexpectedErrorMessage + ex.Message, ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposingStatus)
        {
            if (!isConnectionDisposed)
            {
                isConnectionDisposed = disposingStatus;
            }
        }
    }
}
