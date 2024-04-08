namespace ProdutoCatalogo.Domain.Entities.Token;

public class TokenAccess
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime IssuedAt { get; set; }
}
