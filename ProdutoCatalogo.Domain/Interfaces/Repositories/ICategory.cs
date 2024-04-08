using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.DTOs.Responses;
using ProdutoCatalogo.Domain.Entities.Category;

namespace ProdutoCatalogo.Domain.Interfaces.Repositories;

public interface ICategory
{
    Task<Category> Add(string categoryName, int idUser);
    Task<bool> Update(CategoryUpdate category);
    Task<bool> Delete(int id);
    Task<Category> GetById(int id);
    Task<(IEnumerable<Category> categories, int totalItems)> GetAll(int pageNumber, int pageSize);
    Task<(IEnumerable<Product> products, int totalItems)> GetProductsByCategory(int id, int pageNumber, int pageSize);
}
