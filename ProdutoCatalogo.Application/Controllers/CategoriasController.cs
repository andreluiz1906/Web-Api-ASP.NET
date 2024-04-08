using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Application.DTOs;
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
        ///   "permissoes": [
        ///     {
        ///       "id": 1,
        ///       "nome": "string"
        ///     },
        ///     {
        ///       "id": 1,
        ///       "nome": "string"
        ///     }
        ///   ]
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="401">Forbidden: Código de Usuário ou permissão de acesso vinculado ao token está inválido.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpPost]
        public async Task<IActionResult> BusinessLogin([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
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
        /// Este endpoint permite realizar a consulta de todas permissões cadastradas. Esta é uma consulta paginada
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
        ///   "permissoes": [
        ///     {
        ///       "id": 1,
        ///       "nome": "string"
        ///     },
        ///     {
        ///       "id": 1,
        ///       "nome": "string"
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
        public async Task<IActionResult> BusinessLogin([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
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
                /*
                var (permissoes, totalDeItens) = await _iPermissao.GetAll(pagination.pagina, pagination.tamanho);
                if (permissoes == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    var totalizador = PaginationService.CreatePaginationResponse(permissoes, totalDeItens, pagination);
                    var response = new
                    {
                        totalizador,
                        permissoes,
                    };

                    return Ok(response);
                }
                */
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de categorias. Erro: {ex.Message}");
            }
        }
    }
}
