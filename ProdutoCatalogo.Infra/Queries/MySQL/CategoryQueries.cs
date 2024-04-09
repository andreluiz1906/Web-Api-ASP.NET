namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class CategoryQueries
{
    private const string BaseCountPrefixPermissions = $"SELECT COUNT(c.Id) FROM categoria c ";
    private const string BaseCountPrefixProducts = $"SELECT COUNT(p.Id) FROM produto p WHERE p.Id_categoria = @IdCategory";
    
    public static class Count
    {
        public const string All = BaseCountPrefixPermissions;

        public const string ProductsByCategoryId = BaseCountPrefixProducts;
    }

    public static class Insert
    {
        public const string Item = "INSERT INTO categoria(Nome, Id_usuario) VALUES (@Nome, @Id); SELECT LAST_INSERT_ID();";
    }

    public static class Update
    {
        public const string Item = "UPDATE categoria SET Nome = @Nome WHERE Id = @Id;";
    }

    public static class Delete
    {
        public const string Item = "DELETE FROM categoria WHERE Id = @Id;";
    }

    public static class Get
    {
        public const string All = @$"
{BaseQueries.GetCategory} 
ORDER BY Id
{BaseQueries.CurrentPage};";

        public const string ById = $@"
{BaseQueries.GetCategory} 
AND Id = @Id;";

        public const string ProductsByCategoryId = @$"
{BaseQueries.GetProduct}
AND p.Id_categoria = @IdCategory
{BaseQueries.CurrentPage};";
    }
}
