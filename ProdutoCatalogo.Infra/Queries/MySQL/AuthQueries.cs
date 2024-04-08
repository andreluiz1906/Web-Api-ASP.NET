namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class AuthQueries
{
    public const string GetUser = @$"
{BaseQueries.GetUserPermissionMapping}
AND u.Email = @Email
AND u.Senha = @Senha";
}
