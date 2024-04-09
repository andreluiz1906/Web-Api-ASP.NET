using ProdutoCatalogo.Domain.DTOs.Request;
using ProdutoCatalogo.Domain.DTOs.Responses;

namespace ProdutoCatalogo.Domain.Interfaces.Repositories;

public interface IAuth
{
    Task<User> Login(AuthenticationLogin data);
}
