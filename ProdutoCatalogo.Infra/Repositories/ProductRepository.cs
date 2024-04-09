using Dapper;
using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.DTOs.Responses;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Infra.Interfaces;
using ProdutoCatalogo.Infra.Queries.MySQL;

namespace ProdutoCatalogo.Infra.Repositories;

public class ProductRepository : IProduct
{
    private readonly IConnectionMySQL _connection;
    public ProductRepository(IConnectionMySQL connection)
    {
        _connection = connection;
    }

    public async Task<Product> Add(ProductInsert product, int idUser)
    {
        string sql = ProductQueries.Insert.Item;
        string sqlItem = ProductQueries.Get.ById;
        using (var conn = await _connection.Create())
        {
            try
            {
                int idItem = await conn.ExecuteScalarAsync<int>(sql, new
                {
                    Nome = product.Nome,
                    Descricao = product.Descricao,
                    ValorVenda = product.ValorVenda,
                    ValorCompra = product.ValorCompra,
                    IdCategoria = product.IdCategoria,
                    idUser = idUser
                });

                return await conn.QueryFirstAsync<Product>(sqlItem, new { Id = idItem });
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("Nome_UNIQUE"))
                {
                    error = $"Já existe uma categoria cadastrada com o nome '{product.Nome}'";
                }

                if (error.Contains("fk_Produto_Categoria1"))
                {
                    error = $"Não foi possível localizar uma categoria com o id {product.IdCategoria}";
                }
                throw new Exception(error);
            }
        }
    }

    public async Task<bool> Update(ProductUpdate product)
    {
        string sql = ProductQueries.Update.Item;
        using (var conn = await _connection.Create())
        {
            try
            {
                var updated = await conn.ExecuteAsync(sql, new
                {
                    Nome = product.Nome,
                    Descricao = product.Descricao,
                    ValorVenda = product.ValorVenda,
                    ValorCompra = product.ValorCompra,
                    IdCategoria = product.IdCategoria,
                    Id = product.Id
                });

                return updated > 0;
            }
            catch (MySqlConnector.MySqlException e)
            {
                string error = e.Message;
                if (error.Contains("Nome_UNIQUE"))
                {
                    error = $"Já existe uma categoria cadastrada com o nome '{product.Nome}' atrelado à outro ID";
                }

                if (error.Contains("fk_Produto_Categoria1"))
                {
                    error = $"Não foi possível localizar uma categoria com o id {product.IdCategoria}";
                }

                throw new Exception(error);
            }
        }
    }

    public async Task<bool> Delete(int id)
    {
        string sql = ProductQueries.Delete.Item;
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

    public async Task<Product> GetById(int id)
    {
        string sql = ProductQueries.Get.ById;
        using (var conn = await _connection.Create())
        {
            return await conn.QueryFirstAsync<Product>(sql, new { Id = id });
        }
    }

    public async Task<(IEnumerable<Product> products, int totalItems)> GetAll(int pageNumber, int pageSize)
    {
        string sql = ProductQueries.Get.All;
        string sqlCount = ProductQueries.Count.All;

        IEnumerable<Product> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount);

            using (var multi = await conn.QueryMultipleAsync(sql, new { Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<Product>()).ToList();
            }

            return (results, totalItems);
        }
    }

    public async Task<(IEnumerable<Product> products, int totalItems)> GetProductsByNameOrDescription(string nameProduct, string descriptionProduct, int pageNumber, int pageSize)
    {
        string sqlCondition = string.Empty;
        if(!string.IsNullOrEmpty(nameProduct) && !string.IsNullOrEmpty(descriptionProduct))
        {
            sqlCondition = @$"
(LOWER(p.Nome) COLLATE utf8mb4_general_ci LIKE LOWER('%{nameProduct}%') 
OR LOWER(p.Descricao) COLLATE utf8mb4_general_ci LIKE LOWER('%{descriptionProduct}%')) ";
        }
        else
        {
            if (!string.IsNullOrEmpty(nameProduct))
            {
                sqlCondition = @$" LOWER(p.Nome) COLLATE utf8mb4_general_ci LIKE LOWER('%{nameProduct}%') ";
            }
            else
            {
                sqlCondition = @$" LOWER(p.Descricao) COLLATE utf8mb4_general_ci LIKE LOWER('%{descriptionProduct}%') ";
            }
        }

        string sql = ProductQueries.Get.ByNameOrDescription.Replace("#ParamSearch#", sqlCondition);
        string sqlCount = ProductQueries.Count.ByNameOrDescription.Replace("#ParamSearch#", sqlCondition);

        IEnumerable<Product> results;
        using (var conn = await _connection.Create())
        {
            int pagina = (pageNumber - 1) * pageSize;
            int tamanho = pageSize;
            int totalItems = await conn.ExecuteScalarAsync<int>(sqlCount);

            using (var multi = await conn.QueryMultipleAsync(sql, new { Offset = pagina, Limit = tamanho }))
            {
                results = (await multi.ReadAsync<Product>()).ToList();
            }

            return (results, totalItems);
        }
    }
}
