using System.Data;

namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class PermissionQueries
{
    private const string BaseCountPrefixPermissions = $"SELECT COUNT(p.Id) FROM permissao p ";
    private const string BaseCountPrefixUsers = $"SELECT COUNT(u.Id) FROM usuario u WHERE u.Id_permissao = @IdPermission";
    private const string BaseCountSuffixCount = $" WHERE LOWER(p.Nome) COLLATE utf8mb4_general_ci LIKE LOWER('%#Search#%') ";

    public static class Count
    {
        public const string All = BaseCountPrefixPermissions;

        public const string ByName = $"{BaseCountPrefixPermissions}{BaseCountSuffixCount}";

        public const string UsersByPermissionId = BaseCountPrefixUsers;
    }

    public static class Get
    {
        public const string All = @$"
{BaseQueries.GetPermission} 
ORDER BY Id
{BaseQueries.CurrentPage};";

        public const string ById = $@"
{BaseQueries.GetPermission} 
AND Id = @IdPermission
{BaseQueries.CurrentPage};";

        public const string ByName = $@"
{BaseQueries.GetPermission} 
AND LOWER(Nome) COLLATE utf8mb4_general_ci LIKE LOWER('%#Search#%')
ORDER BY Id
{BaseQueries.CurrentPage};";

        public const string UsersByPermissionId = @$"
{BaseQueries.GetUserPermissionMapping}
AND p.Id = @IdPermission
{BaseQueries.CurrentPage};";
    }
}
