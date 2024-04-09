using Dapper;
using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.DTOs.Responses;
using ProdutoCatalogo.Domain.Entities.Category;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Infra.Interfaces;
using ProdutoCatalogo.Infra.Queries.MySQL;

namespace ProdutoCatalogo.Infra.Repositories;

public class CategoryRepository : ICategory
{
    private readonly IConnectionMySQL _connection;
    public CategoryRepository(IConnectionMySQL connection)
    {
        _connection = connection;
    }

    public async Task<Category> Add(string categoryName, int idUser)
    {
        string sql = CategoryQueries.Insert.Item;
        string sqlItem = CategoryQueries.Get.ById;
        using (var conn = await _connection.Create())
        {
            try
            {
                int idItem = await conn.ExecuteScalarAsync<int>(sql, new { Nome = categoryName, Id = idUser });
                return await conn.QueryFirstAsync<Category>(sqlItem, new { Id = idItem });
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("categoria.nome_UNIQUE"))
                {
                    error = $"Já existe uma categoria cadastrada com o nome '{categoryName}'";
                }

                throw new Exception(error);
            }
        }
    }

    public async Task<bool> Update(CategoryUpdate category)
    {
        string sql = CategoryQueries.Update.Item;
        using (var conn = await _connection.Create())
        {
            try
            {
                var updated = await conn.ExecuteAsync(sql, new { Nome = category.Nome, Id = category.Id });

                return updated > 0;
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("categoria.nome_UNIQUE"))
                {
                    error = $"Já existe uma categoria cadastrada com o nome '{category.Nome}' atrelado à outro ID";
                }

                throw new Exception(error);
            }
        }
    }

    public async Task<bool> Delete(int id)
    {
        string sql = CategoryQueries.Delete.Item;
        using (var conn = await _connection.Create())
        {
            try
            {
                var deleted = await conn.ExecuteAsync(sql, new { Id = id });

                return deleted > 0;
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("foreign key constraint fails"))
                {
                    error = $"Esta categoria não pode ser excluída pois está vinculada a um ou mais produto(s).";
                }

                throw new Exception(error);
            }
        }
    }

    public async Task<Category> GetById(int id)
    {
        string sql = CategoryQueries.Get.ById;
        using (var conn = await _connection.Create())
        {
            return await conn.QueryFirstAsync<Category>(sql, new { Id = id });
        }
    }

    public async Task<(IEnumerable<Category> categories, int totalItems)> GetAll(int pageNumber, int pageSize)
    {
        string sql = CategoryQueries.Get.All;
        string sqlCount = CategoryQueries.Count.All;

        IEnumerable<Category> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount);

            using (var multi = await conn.QueryMultipleAsync(sql, new { Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<Category>()).ToList();
            }

            return (results, totalItems);
        }
    }

    public async Task<(IEnumerable<Product> products, int totalItems)> GetProductsByCategory(int id, int pageNumber, int pageSize)
    {
        string sql = CategoryQueries.Get.ProductsByCategoryId;
        string sqlCount = CategoryQueries.Count.ProductsByCategoryId;

        IEnumerable<Product> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount, new { IdCategory = id });

            using (var multi = await conn.QueryMultipleAsync(sql, new { IdCategory = id, Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<Product>()).ToList();
            }

            return (results, totalItems);
        }
    }
}
