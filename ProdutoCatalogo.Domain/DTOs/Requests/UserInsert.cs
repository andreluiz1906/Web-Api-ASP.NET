using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Domain.DTOs.Requests;

public class UserInsert
{
    [Required(ErrorMessage = ValidationMessages.User.Nickname.Missing)]
    [StringLength(50, ErrorMessage = ValidationMessages.User.Nickname.Size)]
    public string Apelido { get; set; }

    [Required(ErrorMessage = ValidationMessages.User.Email.Missing)]
    [StringLength(255, ErrorMessage = ValidationMessages.User.Email.Size)]
    public string Email { get; set; }

    [Required(ErrorMessage = ValidationMessages.User.Password.Missing)]
    [StringLength(900, ErrorMessage = ValidationMessages.User.Password.Size)]
    public string Senha { get; set; }

    [Required(ErrorMessage = ValidationMessages.User.Permission.Missing)]
    public int IdPermissao { get; set; }
}
