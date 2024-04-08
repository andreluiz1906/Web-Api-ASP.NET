using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Shared.Messages;

namespace ProdutoCatalogo.Application.Configurations.Services;

public class TokenAccessValidator
{
    private readonly IJwtService _jwt;

    public TokenAccessValidator(IJwtService jwtService)
    {
        _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    }
    public ActionResult? ValidateTokenAndHeaders(string headerAuthorization, DateTime headerTimestamp)
    {
        if (string.IsNullOrEmpty(headerAuthorization))
            return new BadRequestObjectResult(ValidationMessages.Header.Authorization.Invalid);

        var now = DateTime.UtcNow;
        var difference = Math.Abs((now - headerTimestamp).TotalMinutes);
        if (difference > 1)
            return new BadRequestObjectResult(ValidationMessages.Header.RequestTimestamp.Expired);

        return null;
    }
}
