namespace ProdutoCatalogo.Domain.Entities.User;

public abstract class UserBase : BaseEntity
{
    public string Apelido { get; set; }
    public string Email { get; set; }
}
