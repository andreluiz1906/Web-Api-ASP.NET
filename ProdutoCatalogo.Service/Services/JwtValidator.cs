using Microsoft.IdentityModel.Tokens;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Shared.Configurations;
using ProdutoCatalogo.Shared.Messages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProdutoCatalogo.Service.Services;

public class JwtValidator : IJwtValidator
{
    private readonly JwtSettings _jwtSettings;

    public JwtValidator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return null;

            token = token.Substring("Bearer ".Length).Trim();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException(ValidationMessages.Token.Invalid);

            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool ValidateClaim(string? token, string? claim, int? valueToCompare)
    {
        if (token == null || claim == null || valueToCompare == null)
            throw new Exception(ValidationMessages.Token.Invalid);

        var retJWT = GetClaimsFromToken(token);
        if (retJWT == null)
            throw new Exception(ValidationMessages.Token.Invalid);

        claim = claim.Trim().ToLower();

        var claimResult = retJWT.Claims.FirstOrDefault(x => x.Type.Trim().ToLower().Equals(claim));
        if (claimResult == null)
            throw new Exception(ValidationMessages.Token.Invalid);

        if (string.IsNullOrEmpty(claimResult.Value))
            throw new Exception(ValidationMessages.Token.Invalid);

        int valueClaim = -1;
        try
        {
            valueClaim = Convert.ToInt32(claimResult.Value);
        }
        catch (Exception)
        {
            valueClaim = -1;
        }
        return valueClaim == valueToCompare.Value;
    }
}

