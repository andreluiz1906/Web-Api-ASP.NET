namespace ProdutoCatalogo.Domain.Entities.Product;

public abstract class ProductBase
{
    public abstract string Nome { get; set; }
    public abstract string Descricao { get; set; }
    public abstract decimal ValorVenda { get; set; }
    public abstract decimal ValorCompra { get; set; }
    public abstract int IdCategoria { get; set; }
}
