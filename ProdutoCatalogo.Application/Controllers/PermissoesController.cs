using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Infra.Configurations.Headers;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Domain.DTOs.Request;
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
        /// Gera o token de acesso aos endpoints.
        /// </summary>
        /// <remarks>
        /// Exemplo de JSON para login:
        /// <pre>
        /// {
        ///   "email": "user@example.com",
        ///   "senha": "string"
        /// }
        /// </pre>
        /// </remarks>
        /// <response code="200">OK: Os dados informados são válidos e o token de acesso foi gerado corretamente.
        /// <pre>
        /// {
        ///   "usuario": {
        ///     "permissao": "string",
        ///     "apelido": "string",
        ///     "email": "user@example.com",
        ///     "id": 1,
        ///     "dataCadastro": "1900-01-01T00:00:00"
        ///   },
        ///   "tokenAcesso": {
        ///     "token": "string",
        ///     "expiration": "1900-01-01T00:00:00",
        ///     "issuedAt": "1900-01-01T00:00:00"
        ///   }
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="404">Not Found: Não foi possível encontrar um usuário com os dados fornecidos.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        //[Authorize]
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
        /// Gera o token de acesso aos endpoints.
        /// </summary>
        /// <remarks>
        /// Exemplo de JSON para login:
        /// <pre>
        /// {
        ///   "email": "user@example.com",
        ///   "senha": "string"
        /// }
        /// </pre>
        /// </remarks>
        /// <response code="200">OK: Os dados informados são válidos e o token de acesso foi gerado corretamente.
        /// <pre>
        /// {
        ///   "usuario": {
        ///     "permissao": "string",
        ///     "apelido": "string",
        ///     "email": "user@example.com",
        ///     "id": 1,
        ///     "dataCadastro": "1900-01-01T00:00:00"
        ///   },
        ///   "tokenAcesso": {
        ///     "token": "string",
        ///     "expiration": "1900-01-01T00:00:00",
        ///     "issuedAt": "1900-01-01T00:00:00"
        ///   }
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="404">Not Found: Não foi possível encontrar um usuário com os dados fornecidos.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        //[Authorize]
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
        /// Gera o token de acesso aos endpoints.
        /// </summary>
        /// <remarks>
        /// Exemplo de JSON para login:
        /// <pre>
        /// {
        ///   "email": "user@example.com",
        ///   "senha": "string"
        /// }
        /// </pre>
        /// </remarks>
        /// <response code="200">OK: Os dados informados são válidos e o token de acesso foi gerado corretamente.
        /// <pre>
        /// {
        ///   "usuario": {
        ///     "permissao": "string",
        ///     "apelido": "string",
        ///     "email": "user@example.com",
        ///     "id": 1,
        ///     "dataCadastro": "1900-01-01T00:00:00"
        ///   },
        ///   "tokenAcesso": {
        ///     "token": "string",
        ///     "expiration": "1900-01-01T00:00:00",
        ///     "issuedAt": "1900-01-01T00:00:00"
        ///   }
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="404">Not Found: Não foi possível encontrar um usuário com os dados fornecidos.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        //[Authorize]
        [HttpGet("nome/{nomepermissao}")]
        public async Task<IActionResult> BusinessLogin([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                       [FromQuery] Pagination pagination,
                                                       string nomepermissao)
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
                var (permissoes, totalDeItens) = await _iPermissao.GetByName(nomepermissao, pagination.pagina, pagination.tamanho);
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
        /// Gera o token de acesso aos endpoints.
        /// </summary>
        /// <remarks>
        /// Exemplo de JSON para login:
        /// <pre>
        /// {
        ///   "email": "user@example.com",
        ///   "senha": "string"
        /// }
        /// </pre>
        /// </remarks>
        /// <response code="200">OK: Os dados informados são válidos e o token de acesso foi gerado corretamente.
        /// <pre>
        /// {
        ///   "usuario": {
        ///     "permissao": "string",
        ///     "apelido": "string",
        ///     "email": "user@example.com",
        ///     "id": 1,
        ///     "dataCadastro": "1900-01-01T00:00:00"
        ///   },
        ///   "tokenAcesso": {
        ///     "token": "string",
        ///     "expiration": "1900-01-01T00:00:00",
        ///     "issuedAt": "1900-01-01T00:00:00"
        ///   }
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="404">Not Found: Não foi possível encontrar um usuário com os dados fornecidos.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        //[Authorize]
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
