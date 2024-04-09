namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class ProductQueries
{
    private const string BaseCountPrefixPermissions = $"SELECT COUNT(p.Id) FROM produto p ";
    
    public static class Count
    {
        public const string All = BaseCountPrefixPermissions;

        public const string ByNameOrDescription = $"{BaseCountPrefixPermissions}WHERE #ParamSearch#";
    }

    public static class Insert
    {
        public const string Item = "INSERT INTO produto(Nome, Descricao, Valor_venda, Valor_compra, Id_categoria, Id_usuario) VALUES (@Nome, @Descricao, @ValorVenda, @ValorCompra, @IdCategoria, @idUser); SELECT LAST_INSERT_ID();";
    }

    public static class Update
    {
        public const string Item = "UPDATE produto SET Nome=@Nome, Descricao=@Descricao, Valor_venda=@ValorVenda, Valor_compra=@ValorCompra, Id_categoria=@IdCategoria WHERE Id=@Id;";
    }

    public static class Delete
    {
        public const string Item = "DELETE FROM produto WHERE Id = @Id;";
    }

    public static class Get
    {
        public const string All = @$"
{BaseQueries.GetProduct} 
ORDER BY Id
{BaseQueries.CurrentPage};";

        public const string ById = $@"
{BaseQueries.GetProduct} 
AND Id = @Id;";

        public const string ByNameOrDescription = @$"
{BaseQueries.GetProduct}
AND #ParamSearch#
{BaseQueries.CurrentPage};";
    }
}
