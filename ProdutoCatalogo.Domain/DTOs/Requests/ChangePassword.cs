using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class ChangePassword
{
    [Required(ErrorMessage = ValidationMessages.User.Password.Missing)]
    [StringLength(900, ErrorMessage = ValidationMessages.User.Password.Size)]
    public string Senha { get; set; }
}
