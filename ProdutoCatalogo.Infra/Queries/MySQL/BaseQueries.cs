namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class BaseQueries
{
	public const string CurrentPage = " LIMIT @Limit OFFSET @Offset;";
	public const string GetPermission = @"
SELECT	* 
FROM	permissao 
WHERE	1 = 1 ";
	public const string GetUserPermissionMapping = @"
SELECT  u.id,
		u.Data_cadastro AS dataCadastro,
		u.Apelido,
		u.Email,
		u.Id_permissao as idPermissao,
		p.Nome AS 'permissao'
FROM	usuario u 
INNER JOIN permissao p ON p.Id = u.Id_permissao 
WHERE 1 = 1 ";
	public const string GetCategory = @"
SELECT c.Id,
	   c.Nome,
	   c.Id_usuario AS IdUsuario 
FROM   categoria c 
WHERE  1 = 1";
	public const string GetProduct = @"
SELECT p.Id, 
	   p.Nome,
	   p.Descricao,
	   p.Valor_venda AS ValorVenda,
	   p.Valor_compra AS ValorCompra,
	   p.data_cadastro AS DataCadastro,
	   p.Id_categoria AS IdCategoria,
	   p.Id_usuario AS IdUsuarioCadastrou
FROM   produto p
WHERE 1 = 1";
}