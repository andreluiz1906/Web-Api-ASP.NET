using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProdutoCatalogo.Infra.Configurations.Swagger;

public class SwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var descricao = @"**Paginação**

Todos os endpoints da API que retornam uma lista de itens são paginados. Para navegar entre as páginas há 02 parâmetros:
| Header        | Descrição                                                                                                                                   |
| ------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| **pagina**    | Representa o número da página a qual deseja consultar                                                                                       |
| **tamanho**   | Quantidade de itens que será retornado pela consulta, os valores permitidos são: 10, 50, 100 ou 150. O valor padrão são 50 itens por página |

&nbsp;

**Controle e Validação**
&nbsp;
| Header                                        | Descrição                                                                                                           |
| --------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| **x-request-timestamp**                         | Parâmetro para validação de requisição solicitada. O valor contido nesse campo deverá ser a data e hora UTC atual |

";
        swaggerDoc.Info.Description = descricao;
    }
}
