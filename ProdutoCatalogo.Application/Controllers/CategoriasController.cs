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
    public class CategoriasController : ControllerBase
    {
        private readonly IJwtService _jwt;
        private readonly ICategory _iCategory;
        public CategoriasController(IJwtService jwtService, ICategory iCategory)
        {
            _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _iCategory = iCategory ?? throw new ArgumentNullException(nameof(iCategory));
        }

        /// <summary>
        /// Este endpoint permite adicionar um novo registro de categoria. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi salvo com sucesso.
        /// <pre>
        /// {
        ///   "idUsuario": 1,
        ///   "id": 1,
        ///   "nome": "string"
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
                                             [FromBody] CategoryInsert model)
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
            if(idUser < 1)
            {
                return StatusCode(403, ValidationMessages.Header.Authorization.OwnerInvalid);
            }

            if(!_jwt.isAdministrator(headerAuthorization))
            {
                return StatusCode(403, ValidationMessages.Header.Authorization.RoleInvalid);
            }

            try
            {
                var categoria = await _iCategory.Add(model.Nome, idUser);
                if(categoria == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    return Ok(categoria);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao incluir cadastro de categoria. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite alterar um registro de categoria. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi atualizado com sucesso.</response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="403">Forbidden: Código de Usuário ou permissão de acesso vinculado ao token está inválido.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                [FromBody] CategoryUpdate model)
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
                var wasUpdated = await _iCategory.Update(model);
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
                return StatusCode(500, $"Falha ao alterar cadastro de categoria. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta de todas as categorias cadastradas. Esta é uma consulta paginada
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
        ///   "categorias": [
        ///     {
        ///       "id": 1,
        ///       "nome": "string",
        ///       "idUsuario": 1
        ///     },
        ///     {
        ///       "id": 1,
        ///       "nome": "string",
        ///       "idUsuario": 1
        ///     }
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
                var (categorias, totalDeItens) = await _iCategory.GetAll(pagination.pagina, pagination.tamanho);
                if (categorias == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    var totalizador = PaginationService.CreatePaginationResponse(categorias, totalDeItens, pagination);
                    var response = new
                    {
                        totalizador,
                        categorias,
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de categorias. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta dos produtos que estão vinculados à categoria pesquisada. Esta é uma consulta paginada
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
        [HttpGet("{id}/produtos")]
        public async Task<IActionResult> GetProductsByCategory([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
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
                var (produtos, totalDeItens) = await _iCategory.GetProductsByCategory(id, pagination.pagina, pagination.tamanho);
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
        /// Este endpoint permite realizar a consulta de uma categoria específica realizando a busca pelo Id.
        /// </summary>
        /// <response code="200">OK: A consulta retorna com os detalhes do item.
        /// <pre>
        /// {
        ///   "id": 1,
        ///   "nome": "string",
        ///   "idUsuario": 1
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
                var permissao = await _iCategory.GetById(id);
                if (permissao == null || permissao.Id < 1)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    return Ok(permissao);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de categoria. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a exclusão de uma categoria específica.
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
                var wasDeleted = await _iCategory.Delete(id);
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
                return StatusCode(500, $"Falha ao remover cadastro de categoria. Erro: {ex.Message}");
            }
        }
    }
}
