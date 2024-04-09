using ProdutoCatalogo.Domain.Entities.User;

namespace ProdutoCatalogo.Domain.DTOs.Responses;

public class User : UserBase
{
    public int idPermissao { get; set; }
    public string Permissao { get; set; }
}
