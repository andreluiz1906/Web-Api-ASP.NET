using MySqlConnector;
using ProdutoCatalogo.Infra.Interfaces;

namespace ProdutoCatalogo.Infra.DataAccess;

public class ConnectionMySQL : IConnectionMySQL
{
    private readonly string _connectionString;

    public ConnectionMySQL(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<MySqlConnection> Create()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
