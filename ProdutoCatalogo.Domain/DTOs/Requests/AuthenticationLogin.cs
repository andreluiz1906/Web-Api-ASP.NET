using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Request;

public class AuthenticationLogin
{
    [Required(ErrorMessage = ValidationMessages.User.Email.Missing)]
    [EmailAddress(ErrorMessage = ValidationMessages.User.Email.Invalid)]
    public string Email { get; set; }

    [Required(ErrorMessage = ValidationMessages.User.Password.Missing)]
    public string Senha { get; set; }
}
