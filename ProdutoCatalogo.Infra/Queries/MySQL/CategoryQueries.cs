namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class CategoryQueries
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
    public static class Insert
    {
        public const string Item = $"INSERT INTO categoria(Nome, Id_usuario) VALUES (@Nome, @Id); SELECT LAST_INSERT_ID();";
    }
    public static class Get
    {
        public const string All = @$"
{BaseQueries.GetCategory} 
ORDER BY Id
{BaseQueries.CurrentPage};";

        public const string ById = $@"
{BaseQueries.GetCategory} 
AND Id = @IdPermission;";

        public const string UsersByPermissionId = @$"
{BaseQueries.GetUserPermissionMapping}
AND p.Id = @IdPermission
{BaseQueries.CurrentPage};";
    }
}
