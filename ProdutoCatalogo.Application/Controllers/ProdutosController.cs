using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Application.DTOs;
using ProdutoCatalogo.Application.Helpers;
using ProdutoCatalogo.Domain.DTOs.Requests;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Infra.Configurations.Headers;
using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(HeaderValidationFilter))]
    [Authorize]
    public class ProdutosController : ControllerBase
    {
        private readonly IJwtService _jwt;
        private readonly IProduct _iProduct;
        public ProdutosController(IJwtService jwtService, IProduct iProduct)
        {
            _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _iProduct = iProduct ?? throw new ArgumentNullException(nameof(iProduct));
        }

        /// <summary>
        /// Este endpoint permite adicionar um novo registro de produto. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi salvo com sucesso.
        /// <pre>
        /// {
        ///    "nome": "string",
        ///    "descricao": "string",
        ///    "valorVenda": 0.00,
        ///    "valorCompra": 0.00,
        ///    "idCategoria": 1,
        ///    "id": 1,
        ///    "dataCadastro": "1900-01-01T00:00:00",
        ///    "idUsuarioCadastrou": 1
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="403">Forbidden: Código de Usuário ou permissão de acesso vinculado ao token está inválido.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpPost]
        public async Task<IActionResult> Add([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                             [FromBody] ProductInsert model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            if (_jwt.IsExpired(headerAuthorization))
            {
                return Forbid(ValidationMessages.Token.Expired);
            }

            int idUser = _jwt.GetOwner(headerAuthorization);
            if (idUser < 1)
            {
                return StatusCode(403, ValidationMessages.Header.Authorization.OwnerInvalid);
            }

            if (!_jwt.isAdministrator(headerAuthorization))
            {
                return StatusCode(403, ValidationMessages.Header.Authorization.RoleInvalid);
            }

            try
            {
                var produto = await _iProduct.Add(model, idUser);
                if (produto == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    return Ok(produto);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao incluir cadastro de produto. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite alterar um registro de produto. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi atualizado com sucesso.</response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="403">Forbidden: Código de Usuário ou permissão de acesso vinculado ao token está inválido.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                [FromBody] ProductUpdate model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            if (_jwt.IsExpired(headerAuthorization))
            {
                return Forbid(ValidationMessages.Token.Expired);
            }

            int idUser = _jwt.GetOwner(headerAuthorization);
            if (idUser < 1)
            {
                return StatusCode(403, ValidationMessages.Header.Authorization.OwnerInvalid);
            }

            if (!_jwt.isAdministrator(headerAuthorization))
            {
                return StatusCode(403, ValidationMessages.Header.Authorization.RoleInvalid);
            }

            try
            {
                var wasUpdated = await _iProduct.Update(model);
                if (!wasUpdated)
                {
                    return NotFound(ValidationMessages.GenericReturnBadRequest);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao alterar cadastro de produto. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a exclusão de um produto específico.
        /// </summary>
        /// <response code="204">No Content: O cadastro foi com sucesso.</response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            if (_jwt.IsExpired(headerAuthorization))
            {
                return Forbid(ValidationMessages.Token.Expired);
            }

            try
            {
                var wasDeleted = await _iProduct.Delete(id);
                if (!wasDeleted)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao remover cadastro de produto. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta de um produto específico realizando a busca pelo Id.
        /// </summary>
        /// <response code="200">OK: A consulta retorna com os detalhes do item.
        /// <pre>
        /// {
        ///    "nome": "string",
        ///    "descricao": "string",
        ///    "valorVenda": 0.00,
        ///    "valorCompra": 0.00,
        ///    "idCategoria": 1,
        ///    "id": 1,
        ///    "dataCadastro": "1900-01-01T00:00:00",
        ///    "idUsuarioCadastrou": 1
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                 [FromQuery] Pagination pagination,
                                                 int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            if (_jwt.IsExpired(headerAuthorization))
            {
                return Forbid(ValidationMessages.Token.Expired);
            }

            try
            {
                var produto = await _iProduct.GetById(id);
                if (produto == null || produto.Id < 1)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    return Ok(produto);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de produto. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta de todos os produtos cadastrados. Esta é uma consulta paginada
        /// </summary>
        /// <response code="200">OK: A consulta retorna a lista com os dados obtidos.
        /// <pre>
        /// {
        ///   "totalizador": {
        ///     "existeMaisPaginas": true,
        ///     "paginaAtual": 1,
        ///     "totalDePaginas": 1,
        ///     "totalDeItens": 1,
        ///     "itensDaLista": 1,
        ///     "limiteDeItens": 50
        ///   },
        ///   "produtos": [
        ///   {
        ///      "nome": "string",
        ///      "descricao": "string",
        ///      "valorVenda": 0.00,
        ///      "valorCompra": 0.00,
        ///      "idCategoria": 1,
        ///      "id": 1,
        ///      "dataCadastro": "1900-01-01T00:00:00",
        ///      "idUsuarioCadastrou": 1
        ///   },
        ///   {
        ///      "nome": "string",
        ///      "descricao": "string",
        ///      "valorVenda": 0.00,
        ///      "valorCompra": 0.00,
        ///      "idCategoria": 1,
        ///      "id": 1,
        ///      "dataCadastro": "1900-01-01T00:00:00",
        ///      "idUsuarioCadastrou": 1
        ///   }
        ///   ]
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                [FromQuery] Pagination pagination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            if (_jwt.IsExpired(headerAuthorization))
            {
                return Forbid(ValidationMessages.Token.Expired);
            }

            try
            {
                var (produtos, totalDeItens) = await _iProduct.GetAll(pagination.pagina, pagination.tamanho);
                if (produtos == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    var totalizador = PaginationService.CreatePaginationResponse(produtos, totalDeItens, pagination);
                    var response = new
                    {
                        totalizador,
                        produtos,
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de produtos. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta dos produtos filtrando a busca pelo nome ou descrição. Esta é uma consulta paginada
        /// </summary>
        /// <response code="200">OK: A consulta retorna a lista com os dados obtidos.
        /// <pre>
        /// {
        ///   "totalizador": {
        ///     "existeMaisPaginas": true,
        ///     "paginaAtual": 1,
        ///     "totalDePaginas": 1,
        ///     "totalDeItens": 1,
        ///     "itensDaLista": 1,
        ///     "limiteDeItens": 50
        ///   },
        ///   "produtos": [
        ///     {
        ///       "id": 1,
        ///       "idCategoria": 1,
        ///       "idUsuarioCadastrou": 1,
        ///       "nome": "Aparelho Medidor de Pressão",
        ///       "descricao": "Aparelho para medir pressão arterial. Digital de pulso",
        ///       "valorVenda": 80.00,
        ///       "valorCompra": 60.00,
        ///       "dataCadastro": "2024-04-05T23:31:28"
        ///     },
        ///     {
        ///       "id": 1,
        ///       "idCategoria": 1,
        ///       "idUsuarioCadastrou": 1,
        ///       "nome": "Aparelho Medidor de Pressão",
        ///       "descricao": "Aparelho para medir pressão arterial. Digital de pulso",
        ///       "valorVenda": 80.00,
        ///       "valorCompra": 60.00,
        ///       "dataCadastro": "2024-04-05T23:31:28"
        ///     }
        ///   ]
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpGet("filtrar")]
        public async Task<IActionResult> GetProductsByCategory([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                               [FromQuery] Pagination pagination,
                                                               [FromQuery] ProductQueryParam filters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            if (string.IsNullOrEmpty(filters.nome) && string.IsNullOrEmpty(filters.descricao))
            {
                return BadRequest(ValidationMessages.EmptyQueryParams);
            }

            if (_jwt.IsExpired(headerAuthorization))
            {
                return Forbid(ValidationMessages.Token.Expired);
            }

            try
            {
                var (produtos, totalDeItens) = await _iProduct.GetProductsByNameOrDescription(filters.nome, filters.descricao, pagination.pagina, pagination.tamanho);
                if (produtos == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    var totalizador = PaginationService.CreatePaginationResponse(produtos, totalDeItens, pagination);
                    var response = new
                    {
                        totalizador,
                        produtos,
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de produtos. Erro: {ex.Message}");
            }
        }
    }
}
