namespace ProdutoCatalogo.Infra.Queries.MySQL;

public static class BaseQueries
{
	public const string CurrentPage = @" LIMIT @Limit OFFSET @Offset;";
	public const string GetPermission = @"
SELECT	* 
FROM	permissao 
WHERE	1 = 1 ";

	public const string GetUserPermissionMapping = @"
SELECT  u.id,
		u.Data_cadastro AS dataCadastro,
		u.Apelido,
		u.Email,
		p.Nome AS 'permissao'
FROM	usuario u 
INNER JOIN permissao p ON p.Id = u.Id_permissao 
WHERE 1 = 1 ";
}