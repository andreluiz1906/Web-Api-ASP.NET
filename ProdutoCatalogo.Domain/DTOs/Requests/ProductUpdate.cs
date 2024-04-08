using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class ProductUpdate : ProductInsert
{
    [Required(ErrorMessage = ValidationMessages.Product.Id.Missing)]
    public int Id { get; set; }
}
