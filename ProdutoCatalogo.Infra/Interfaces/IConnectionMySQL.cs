using MySqlConnector;

namespace ProdutoCatalogo.Infra.Interfaces;

public interface IConnectionMySQL
{
    Task<MySqlConnection> Create();
}
