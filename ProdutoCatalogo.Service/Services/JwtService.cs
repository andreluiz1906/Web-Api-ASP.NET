using Microsoft.IdentityModel.Tokens;
using ProdutoCatalogo.Domain.Entities.Token;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Shared.Configurations;
using ProdutoCatalogo.Shared.Messages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProdutoCatalogo.Service.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    private string GetNameIdentification(string originValue)
    {
        if (string.IsNullOrEmpty(originValue))
            return "Unknown";

        return GetEmailUsername(originValue);
    }

    private string GetEmailUsername(string email)
    {
        int atIndex = email.IndexOf('@');
        if (atIndex > -1)
        {
            return email.Substring(0, atIndex);
        }
        return email;
    }

    public TokenGenerator SetClaim(int id, string nick, string email, string permission)
    {
        return new TokenGenerator
        {
            RoleAccess = permission,
            Requester = nick,
            IdentificationName = email,
            Owner = id
        };
    }
    public int GetOwner(string token)
    {
        var claims = this.GetClaims(token);
        if (claims == null || claims.Count < 1)
            throw new Exception(ValidationMessages.Token.Invalid);

        int.TryParse(GetClaimValue(claims, "Owner"), out int idOwner);

        return idOwner;
    }

    public bool isAdministrator(string token)
    {
        var claims = this.GetClaims(token);
        if (claims == null || claims.Count < 1)
            throw new Exception(ValidationMessages.Token.Invalid);

        string permission = GetClaimValue(claims, "role");

        return permission.Equals("Administrador");
    }

    private TokenGenerator? GetTokenGeneratorFromToken(string token)
    {
        var claims = this.GetClaims(token);
        if (claims == null || claims.Count < 1)
            throw new Exception(ValidationMessages.Token.Invalid);

        string email = GetClaimValue(claims, "unique_name");
        string permission = GetClaimValue(claims, "role");
        string nick = GetClaimValue(claims, "Requester");
        int.TryParse(GetClaimValue(claims, "Owner"), out int idOwner);

        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(permission) || string.IsNullOrEmpty(nick) || idOwner < 1)
            throw new Exception(ValidationMessages.Token.Invalid);

        return new TokenGenerator
        {
            IdentificationName = email,
            RoleAccess = permission,
            Requester = nick,
            Owner = idOwner
        };
    }

    private string GetClaimValue(List<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(claim => claim.Type.ToLower().Equals(claimType.ToLower()))?.Value ?? string.Empty;
    }

    public TokenAccess Generate(TokenGenerator claimsToken)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, claimsToken.IdentificationName),
            new Claim(ClaimTypes.Role, claimsToken.RoleAccess),
            new Claim("Requester", claimsToken.Requester),
            new Claim("Owner", claimsToken.Owner.ToString())
        };

        DateTime dtGenerate = DateTime.UtcNow;
        DateTime dtExpire = dtGenerate.AddMinutes(_jwtSettings.ExpirationInMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = dtExpire,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new TokenAccess
        {
            Expiration = dtExpire,
            IssuedAt = dtGenerate,
            Token = tokenHandler.WriteToken(token)
        };
    }

    public string GetClaimValue(string token, string claimType)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(claimType))
            return string.Empty;

        token = token.Replace("Bearer ", string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var tokenDecoded = handler.ReadJwtToken(token);
        string? valueClaim = tokenDecoded.Claims.FirstOrDefault(claim => claim.Type.ToLower().Equals(claimType.ToLower()))?.Value;

        return (string.IsNullOrEmpty(valueClaim) ? string.Empty : valueClaim.ToString());
    }

    public bool IsExpired(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception(ValidationMessages.Header.Authorization.Invalid);

        token = token.Replace("Bearer ", string.Empty);

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(token);

        var expirationClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        if (expirationClaim != null && long.TryParse(expirationClaim.Value, out long expirationTime))
        {
            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return expirationTime <= currentUnixTime;
        }

        return true;
    }

    public TokenAccess? Refresh(string token)
    {
        var claimsToken = GetTokenGeneratorFromToken(token);
        if (claimsToken == null)
            return null;
        else
            return Generate(claimsToken);
    }

    public List<Claim> GetClaims(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception(ValidationMessages.Header.Authorization.Invalid);

        token = token.Replace("Bearer ", string.Empty);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.ToList();
    }
}
