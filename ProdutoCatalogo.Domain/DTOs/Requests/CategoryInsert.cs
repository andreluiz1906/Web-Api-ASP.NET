using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class CategoryInsert
{
    [Required(ErrorMessage = ValidationMessages.Category.Name.Missing)]
    [StringLength(80, ErrorMessage = ValidationMessages.Category.Name.Size)]
    public string Nome { get; set; }
}
