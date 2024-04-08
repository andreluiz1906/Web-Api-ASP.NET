using ProdutoCatalogo.Domain.DTOs.Request;
using ProdutoCatalogo.Domain.DTOs.Responses;
using ProdutoCatalogo.Infra.Interfaces;
using Dapper;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Infra.Queries.MySQL;

namespace ProdutoCatalogo.Infra.Repositories;

public class AuthRepository : IAuth
{
    private readonly IConnectionMySQL _connection;
    public AuthRepository(IConnectionMySQL connection)
    {
        _connection = connection;
    }

    public async Task<UserPermissionMapping> Login(AuthenticationLogin data)
    {
        string sql = AuthQueries.GetUser;

        using (var conn = await _connection.Create())
        {
            return await conn.QueryFirstOrDefaultAsync<UserPermissionMapping>(sql, data);
        }
    }
}
