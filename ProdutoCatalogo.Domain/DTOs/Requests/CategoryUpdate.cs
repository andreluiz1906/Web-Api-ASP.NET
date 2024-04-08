using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class CategoryUpdate : CategoryInsert
{
    [Required(ErrorMessage = ValidationMessages.Category.Id.Missing)]
    public int Id { get; set; }
}
