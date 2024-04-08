using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Application.Interfaces;
using ProdutoCatalogo.Shared.Messages;

namespace ProdutoCatalogo.Application.Configurations.Services;

public class HttpHeaderService : IHttpHeaderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpHeaderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetHeaderValue(string headerName)
    {
        if (_httpContextAccessor.HttpContext?.Request?.Headers != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(headerName, out var value))
        {
            return value;
        }

        return null;
    }

    public bool IsHeaderTimestamp(DateTime timestampValue)
    {
        var currentUtcDateTime = DateTime.UtcNow;
        var difference = Math.Abs((currentUtcDateTime - timestampValue).TotalMinutes);
        return (difference > 60);
    }
}

