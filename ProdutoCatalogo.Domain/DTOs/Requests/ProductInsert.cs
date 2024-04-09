using ProdutoCatalogo.Domain.Entities.Product;
using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class ProductInsert : ProductBase
{
    [Required(ErrorMessage = ValidationMessages.Product.Name.Missing)]
    [StringLength(100, ErrorMessage = ValidationMessages.Product.Name.Size)]
    public override string Nome { get ; set ; }

    [StringLength(250, ErrorMessage = ValidationMessages.Product.Description.Size)]
    public override string Descricao { get ; set ; }

    [Required(ErrorMessage = ValidationMessages.Product.ValuePrice.Missing)]
    public override decimal ValorVenda { get ; set ; }

    [Required(ErrorMessage = ValidationMessages.Product.PurchasePrice.Missing)]
    public override decimal ValorCompra { get ; set ; }

    [Required(ErrorMessage = ValidationMessages.Product.Category.Missing)]
    public override int IdCategoria { get ; set ; }
}
