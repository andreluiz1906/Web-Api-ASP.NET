using ProdutoCatalogo.Domain.Entities.User;

namespace ProdutoCatalogo.Domain.DTOs.Responses;

public class UserPermissionMapping : UserBase
{
    public string Permissao { get; set; }
}
