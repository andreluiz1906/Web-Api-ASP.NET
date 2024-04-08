using ProdutoCatalogo.Domain.DTOs.Requests;

namespace ProdutoCatalogo.Domain.Entities.Category;

public class Category : CategoryUpdate
{
    public int IdUsuario { get; set; }
}
