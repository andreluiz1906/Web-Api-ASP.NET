
using Dapper;
using ProdutoCatalogo.Domain.DTOs.Responses;
using ProdutoCatalogo.Infra.Interfaces;
using ProdutoCatalogo.Infra.Queries.MySQL;
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProdutoCatalogo.Infra.Repositories;

public class PermissionRepository : Domain.Interfaces.Repositories.IPermission
{
    private readonly IConnectionMySQL _connection;
    public PermissionRepository(IConnectionMySQL connection)
    {
        _connection = connection;
    }


    public async Task<Permission> GetById(int id)
    {
        string sql = PermissionQueries.Get.ById;

        using (var conn = await _connection.Create())
        {
            return await conn.QueryFirstOrDefaultAsync<Permission>(sql, new { IdPermission = id });
        }
    }

    public async Task<(IEnumerable<Permission> permissions, int totalItems)> GetAll(int pageNumber, int pageSize)
    {
        string sql = PermissionQueries.Get.All;
        string sqlCount = PermissionQueries.Count.All;

        IEnumerable<Permission> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount);

            using (var multi = await conn.QueryMultipleAsync(sql, new { Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<Permission>()).ToList();
            }

            return (results, totalItems);
        }
    }

    public async Task<(IEnumerable<Permission> permissions, int totalItems)> GetByName(string name, int pageNumber, int pageSize)
    {
        string sql = PermissionQueries.Get.ByName.Replace("#Search#", name);
        string sqlCount = PermissionQueries.Count.ByName.Replace("#Search#", name);

        IEnumerable<Permission> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount);

            using (var multi = await conn.QueryMultipleAsync(sql, new { Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<Permission>()).ToList();
            }

            return (results, totalItems);
        }
    }

    public async Task<(IEnumerable<UserPermissionMapping> users, int totalItems)> GetUserPermissionsByType(int id, int pageNumber, int pageSize)
    {
        string sql = PermissionQueries.Get.UsersByPermissionId;
        string sqlCount = PermissionQueries.Count.UsersByPermissionId;

        IEnumerable<UserPermissionMapping> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount, new { IdPermission = id });

            using (var multi = await conn.QueryMultipleAsync(sql, new { IdPermission = id, Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<UserPermissionMapping>()).ToList();
            }

            return (results, totalItems);
        }
    }
}
