using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Data.Fakes
{
    public class FakeDataLink : IDataLink
    {
        // Optional dictionary to allow setting up fake results for specific stored procedures.
        private readonly Dictionary<string, DataTable> fakeResults = new();

        public FakeDataLink()
        {
            // Optionally, seed fakeResults with default data.
        }

        public T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            // Example: If the stored procedure is "GetFriendshipCountForUser", simulate returning a count.
            if (storedProcedure == "GetFriendshipCountForUser" && sqlParameters != null)
            {
                int userId = (int)sqlParameters.First(p => p.ParameterName == "@user_id").Value;
                // For example, if userId equals 1, return 2; otherwise return 0.
                object result = (userId == 1) ? 2 : 0;
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return default;
        }

        public DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            // If a fake result is configured for this stored procedure, return it.
            if (fakeResults.ContainsKey(storedProcedure))
            {
                return fakeResults[storedProcedure];
            }
            // Otherwise, return an empty DataTable.
            return new DataTable();
        }

        public int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            // For testing purposes, simply return 1 to indicate a successful operation.
            return 1;
        }

        public async Task<DataTable> ExecuteReaderAsync(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            // For testing, wrap the synchronous call in a Task.
            return await Task.FromResult(ExecuteReader(storedProcedure, sqlParameters));
        }

        public async Task ExecuteNonQueryAsync(string storedProcedure, SqlParameter[]? sqlParameters = null)
        {
            // For testing, simulate async execution.
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            // No resources to dispose in this fake implementation.
        }

        // Optional helper method to allow tests to pre-configure fake results.
        public void SetFakeResult(string storedProcedure, DataTable dataTable)
        {
            fakeResults[storedProcedure] = dataTable;
        }
    }
}
