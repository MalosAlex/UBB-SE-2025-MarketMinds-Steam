using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace BusinessLayer.Data
{
    public interface IDataLink : IDisposable
    {
        T? ExecuteScalar<T>(string storedProcedure, SqlParameter[]? sqlParameters = null);
        DataTable ExecuteReader(string storedProcedure, SqlParameter[]? sqlParameters = null);
        int ExecuteNonQuery(string storedProcedure, SqlParameter[]? sqlParameters = null);
    }
}