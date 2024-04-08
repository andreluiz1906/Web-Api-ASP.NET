using ProdutoCatalogo.Domain.Entities.Product;

namespace ProdutoCatalogo.Domain.DTOs.Responses;

public class Product : ProductBase
{
    public override string Nome { get; set; }
    public override string Descricao { get; set; }
    public override decimal ValorVenda { get; set; }
    public override decimal ValorCompra { get; set; }
    public override int IdCategoria { get; set; }
    public int Id { get; set; }
    public DateTime DataCadastro { get; set; }
    public int IdUsuarioCadastrou { get; set; }
}
