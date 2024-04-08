namespace ProdutoCatalogo.Application.Interfaces;

public interface IHttpHeaderService
{
    string? GetHeaderValue(string headerName);
    bool IsHeaderTimestamp(DateTime timestampValue);
}
