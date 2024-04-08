namespace ProdutoCatalogo.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime DataCadastro { get; set; }
}
