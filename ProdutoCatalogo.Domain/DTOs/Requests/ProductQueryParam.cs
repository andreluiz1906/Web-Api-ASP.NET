using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class ProductQueryParam
{
    [StringLength(100, ErrorMessage = ValidationMessages.Product.Name.Size)]
    public string nome { get; set; }

    [StringLength(250, ErrorMessage = ValidationMessages.Product.Name.Size)]
    public string descricao { get; set; }
}
