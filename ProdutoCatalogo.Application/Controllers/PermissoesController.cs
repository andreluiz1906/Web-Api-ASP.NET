using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Infra.Configurations.Headers;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;
using ProdutoCatalogo.Application.DTOs;
using ProdutoCatalogo.Application.Helpers;

namespace ProdutoCatalogo.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(HeaderValidationFilter))]
    [Authorize]
    public class PermissoesController : ControllerBase
    {
        private readonly IJwtService _jwt;
        private readonly IPermission _iPermissao;
        public PermissoesController(IJwtService jwtService, IPermission iPermissao)
        {
            _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _iPermissao = iPermissao ?? throw new ArgumentNullException(nameof(iPermissao));
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
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de permissão de acesso. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta de uma permissão específica realizando a busca pelo Id.
        /// </summary>
        /// <response code="200">OK: A consulta retorna com os detalhes do item.
        /// <pre>
        ///   {
        ///     "id": 1,
        ///     "nome": "string"
        ///   }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
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
                var permissao = await _iPermissao.GetById(id);
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
                return StatusCode(500, $"Falha ao consultar cadastro de permissão de acesso. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta das permissões cadastradas filtrando a busca pelo nome da permissão. Esta é uma consulta paginada
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
        [HttpGet("nome/{nome}")]
        public async Task<IActionResult> GetByName([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                   [FromQuery] Pagination pagination,
                                                   string nome)
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
                var (permissoes, totalDeItens) = await _iPermissao.GetByName(nome, pagination.pagina, pagination.tamanho);
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
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de permissão de acesso. Erro: {ex.Message}");
            }
        }

        /// <summary>        
        /// Este endpoint permite realizar a consulta dos usuários que estão vinculados à permissão pesquisada. Esta é uma consulta paginada
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
        ///   "usuarios": [
        ///     {
        ///       "id": 1,
        ///       "apelido": "string",
        ///       "email": "user@example.com",
        ///       "idPermissao": 1,
        ///       "permissao": "string",
        ///       "dataCadastro": "1900-01-01T00:00:00"
        ///     },
        ///     {
        ///       "id": 1,
        ///       "apelido": "string",
        ///       "email": "user@example.com",
        ///       "idPermissao": 1,
        ///       "permissao": "string",
        ///       "dataCadastro": "1900-01-01T00:00:00"
        ///     }
        ///   ]
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpGet("{id}/usuarios")]
        public async Task<IActionResult> GetUsers([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
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
                var (usuarios, totalDeItens) = await _iPermissao.GetUserPermissionsByType(id, pagination.pagina, pagination.tamanho);
                if (usuarios == null)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    var totalizador = PaginationService.CreatePaginationResponse(usuarios, totalDeItens, pagination);
                    var response = new
                    {
                        totalizador,
                        usuarios,
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de usuários vinculados à permissão de acesso. Erro: {ex.Message}");
            }
        }
    }
}
