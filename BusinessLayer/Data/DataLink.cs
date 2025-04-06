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
        private readonly string connectionString;
        private bool disposed;

        private DataLink()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string? localDataSource = config["LocalDataSource"];
                string? initialCatalog = config["InitialCatalog"];
                string? userId = config["UserId"];
                string? password = config["Password"];

                if (string.IsNullOrWhiteSpace(localDataSource) || string.IsNullOrWhiteSpace(initialCatalog))
                {
                    throw new ConfigurationErrorsException("Database connection settings are missing in appsettings.json");
                }

                // connectionString = $"Data Source={localDataSource};Initial Catalog={initialCatalog};User Id={userId};Password={password};TrustServerCertificate=True;";
                connectionString = $"Data Source={localDataSource};Initial Catalog={initialCatalog};Integrated Security=True;TrustServerCertificate=True;";

                // Test the connection immediately
                using var testConnection = new SqlConnection(connectionString);
                testConnection.Open();
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException("Failed to establish database connection. Please check your connection settings.", ex);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Failed to initialize database connection.", ex);
            }
        }

        public static DataLink Instance => DataLinkInstance.Value;

        private SqlConnection CreateConnection()
        {
            if (disposed)
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
                throw new DatabaseOperationException($"Database error during ExecuteScalar operation: {ex.Message}", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new DatabaseOperationException($"Error during ExecuteScalar operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Unexpected error during ExecuteScalar operation: {ex.Message}", ex);
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
                throw new DatabaseOperationException($"Database error during ExecuteReader operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error during ExecuteReader operation: {ex.Message}", ex);
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
                throw new DatabaseOperationException($"Database error during ExecuteNonQuery operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error during ExecuteNonQuery operation: {ex.Message}", ex);
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
                throw new DatabaseOperationException($"Database error during ExecuteReaderAsync operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error during ExecuteReaderAsync operation: {ex.Message}", ex);
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
                throw new DatabaseOperationException($"Database error during ExecuteNonQueryAsync operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException($"Error during ExecuteNonQueryAsync operation: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }
    }
}