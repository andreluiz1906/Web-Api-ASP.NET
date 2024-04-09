namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class UserQueries
{
    private const string BaseCountPrefixPermissions = $"SELECT COUNT(u.id) FROM usuario u ";
    
    public static class Count
    {
        public const string All = BaseCountPrefixPermissions;

    }
    public static class Insert
    {
        public const string Item = "INSERT INTO usuario(Apelido, Email, Senha, Id_permissao) VALUES (@Apelido, @Email, @Senha, @IdPermissao); SELECT LAST_INSERT_ID();";
    }

    public static class Update
    {
        public const string Item = "UPDATE usuario SET Apelido=@Apelido, Email=@Email, Id_permissao=@IdPermissao WHERE id =@Id;";

        public const string Password = "UPDATE usuario SET Senha=@Senha WHERE id = @Id;";
    }

    public static class Delete
    {
        public const string Item = "DELETE FROM usuario WHERE id = @Id;";
    }

    public static class Get
    {
        public const string All = @$"
{BaseQueries.GetUserPermissionMapping} 
ORDER BY u.id
{BaseQueries.CurrentPage};";

        public const string ById = $@"
{BaseQueries.GetUserPermissionMapping} 
AND u.id = @Id;";
    }
}
