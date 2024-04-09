using Dapper;
using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.DTOs.Responses;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Infra.Interfaces;
using ProdutoCatalogo.Infra.Queries.MySQL;

namespace ProdutoCatalogo.Infra.Repositories;

public class UserRepository : IUser
{
    private readonly IConnectionMySQL _connection;
    public UserRepository(IConnectionMySQL connection)
    {
        _connection = connection;
    }

    public async Task<User> Add(UserInsert user)
    {
        string sql = UserQueries.Insert.Item;
        string sqlItem = UserQueries.Get.ById;
        using (var conn = await _connection.Create())
        {
            try
            {
                int idItem = await conn.ExecuteScalarAsync<int>(sql, user);
                return await conn.QueryFirstAsync<User>(sqlItem, new { Id = idItem });
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("Email_UNIQUE"))
                {
                    error = $"Já existe um usuário cadastrado com o endereço de e-mail '{user.Email}'";
                }

                if (error.Contains("Apelido_UNIQUE"))
                {
                    error = $"Já existe um usuário cadastrado com o apelido de '{user.Apelido}'";
                }

                if (error.Contains("fk_Usuario_Permissao"))
                {
                    error = $"Não foi possível localizar uma permissão com o id {user.IdPermissao}";
                }

                throw new Exception(error);
            }
        }
    }

    public async Task<bool> Update(UserUpdate user)
    {
        string sql = UserQueries.Update.Item;
        using (var conn = await _connection.Create())
        {
            try
            {
                var updated = await conn.ExecuteAsync(sql, user);

                return updated > 0;
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("Email_UNIQUE"))
                {
                    error = $"Já existe um usuário cadastrado com o endereço de e-mail '{user.Email}' atrelado à outro ID";
                }

                if (error.Contains("Apelido_UNIQUE"))
                {
                    error = $"Já existe um usuário cadastrado com o apelido de '{user.Apelido}' atrelado à outro ID";
                }

                if (error.Contains("fk_Usuario_Permissao"))
                {
                    error = $"Não foi possível localizar uma permissão com o id {user.IdPermissao}";
                }

                throw new Exception(error);
            }
        }
    }

    public async Task<bool> UpdatePassword(string newPassword, int id)
    {
        string sql = UserQueries.Update.Password;
        using (var conn = await _connection.Create())
        {
            var updated = await conn.ExecuteAsync(sql, new { Senha = newPassword, Id = id });

            return updated > 0;
        }
    }

    public async Task<bool> Delete(int id)
    {
        string sql = UserQueries.Delete.Item;
        using (var conn = await _connection.Create())
        {
            var deleted = await conn.ExecuteAsync(sql, new { Id = id });

            return deleted > 0;
        }
    }

    public async Task<User> GetById(int id)
    {
        string sql = UserQueries.Get.ById;
        using (var conn = await _connection.Create())
        {
            return await conn.QueryFirstAsync<User>(sql, new { Id = id });
        }
    }

    public async Task<(IEnumerable<User> users, int totalItems)> GetAll(int pageNumber, int pageSize)
    {
        string sql = UserQueries.Get.All;
        string sqlCount = UserQueries.Count.All;

        IEnumerable<User> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount);

            using (var multi = await conn.QueryMultipleAsync(sql, new { Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<User>()).ToList();
            }

            return (results, totalItems);
        }
    }
}
