using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.DTOs.Responses;

namespace ProdutoCatalogo.Domain.Interfaces.Repositories;

public interface IProduct
{
    Task<Product> Add(ProductInsert product, int idUser);
    Task<bool> Update(ProductUpdate product);
    Task<bool> Delete(int id);
    Task<Product> GetById(int id);
    Task<(IEnumerable<Product> products, int totalItems)> GetAll(int pageNumber, int pageSize);
    Task<(IEnumerable<Product> products, int totalItems)> GetProductsByNameOrDescription(string nameProduct, string descriptionProduct, int pageNumber, int pageSize);
}
