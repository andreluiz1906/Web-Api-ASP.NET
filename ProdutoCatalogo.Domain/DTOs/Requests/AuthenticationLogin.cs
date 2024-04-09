using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Request;

public class AuthenticationLogin
{
    [Required(ErrorMessage = ValidationMessages.User.Email.Missing)]
    [EmailAddress(ErrorMessage = ValidationMessages.User.Email.Invalid)]
    [StringLength(255, ErrorMessage = ValidationMessages.User.Email.Size)]
    public string Email { get; set; }

    [Required(ErrorMessage = ValidationMessages.User.Password.Missing)]
    [StringLength(900, ErrorMessage = ValidationMessages.User.Password.Size)]
    public string Senha { get; set; }
}
