using Microsoft.VisualBasic;
using ProdutoCatalogo.Domain.Entities.Token;
using System.Security.Claims;

namespace ProdutoCatalogo.Domain.Interfaces.Services;

public interface IJwtService
{
    TokenGenerator SetClaim(int id, string nick, string email, string permission);
    TokenAccess Generate(TokenGenerator claimsToken);
    TokenAccess? Refresh(string token);
    List<Claim> GetClaims(string token);
    bool IsExpired(string token);
    string GetClaimValue(string token, string claimType);
    int GetOwner(string token);
    bool isAdministrator(string token);
}
