using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.DTOs.Responses;

namespace ProdutoCatalogo.Domain.Interfaces.Repositories;

public interface IUser
{
    Task<User> Add(UserInsert user);
    Task<bool> Update(UserUpdate user);
    Task<bool> UpdatePassword(string newPassword, int id);
    Task<bool> Delete(int id);
    Task<User> GetById(int id);
    Task<(IEnumerable<User> users, int totalItems)> GetAll(int pageNumber, int pageSize);
}
