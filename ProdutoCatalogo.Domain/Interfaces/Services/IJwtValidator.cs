using System.Security.Claims;

namespace ProdutoCatalogo.Domain.Interfaces.Services;

public interface IJwtValidator
{
    bool ValidateClaim(string? token, string? claim, int? valueToCompare);
    ClaimsPrincipal? GetClaimsFromToken(string token);
}