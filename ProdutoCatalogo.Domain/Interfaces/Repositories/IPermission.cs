using ProdutoCatalogo.Domain.DTOs.Responses;

namespace ProdutoCatalogo.Domain.Interfaces.Repositories;

public interface IPermission
{
    Task<(IEnumerable<Permission> permissions, int totalItems)> GetAll(int pageNumber, int pageSize);
    Task<Permission> GetById(int id);
    Task<(IEnumerable<Permission> permissions, int totalItems)> GetByName(string name, int pageNumber, int pageSize);
    Task<(IEnumerable<User> users, int totalItems)> GetUserPermissionsByType(int id, int pageNumber, int pageSize);
}
