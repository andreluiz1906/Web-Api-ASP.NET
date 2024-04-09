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
    public class UsuariosController : ControllerBase
    {
        private readonly IJwtService _jwt;
        private readonly IUser _iUser;
        public UsuariosController(IJwtService jwtService, IUser iUser)
        {
            _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _iUser = iUser ?? throw new ArgumentNullException(nameof(IUser));
        }

        /// <summary>
        /// Este endpoint permite adicionar um novo registro de usuário. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi salvo com sucesso.
        /// <pre>
        /// {
        ///   "id": 1,
        ///   "apelido": "string",
        ///   "email": "user@example.com",
        ///   "idPermissao": 1,
        ///   "permissao": "string",
        ///   "dataCadastro": "1900-01-01T00:00:00"
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
                                             [FromBody] UserInsert model)
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
                var categoria = await _iUser.Add(model);
                if (categoria == null)
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
                return StatusCode(500, $"Falha ao incluir cadastro de usuário. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite alterar um registro de usuário. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi atualizado com sucesso.</response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="403">Forbidden: Código de Usuário ou permissão de acesso vinculado ao token está inválido.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                [FromBody] UserUpdate model)
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
                var wasUpdated = await _iUser.Update(model);
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
                return StatusCode(500, $"Falha ao alterar cadastro de usuário. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite alterar a senha do cadastro do usuário. Para acessar é necessário que o token esteja atrelado a uma conta com permissão de Administrador
        /// </summary>
        /// <response code="200">OK: O registro foi atualizado com sucesso.</response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="401">Unauthorized: Não foi informado o token de acesso ou o token está expirado.</response>
        /// <response code="403">Forbidden: Código de Usuário ou permissão de acesso vinculado ao token está inválido.</response>
        /// <response code="404">Not Found: Não foi possível encontrar nenhum registro.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [HttpPut("{id}/senha")]
        public async Task<IActionResult> Update([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                [FromBody] ChangePassword model,
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
                var wasUpdated = await _iUser.UpdatePassword(model.Senha, id);
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
                return StatusCode(500, $"Falha ao alterar senha do cadastro de usuário. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a exclusão de um usuário específico.
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
                var wasDeleted = await _iUser.Delete(id);
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
                return StatusCode(500, $"Falha ao remover cadastro de usuário. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta de um usuário específico realizando a busca pelo Id.
        /// </summary>
        /// <response code="200">OK: A consulta retorna com os detalhes do item.
        /// <pre>
        /// {
        ///   "id": 1,
        ///   "apelido": "string",
        ///   "email": "user@example.com",
        ///   "idPermissao": 1,
        ///   "permissao": "string",
        ///   "dataCadastro": "1900-01-01T00:00:00"
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
                var usuario = await _iUser.GetById(id);
                if (usuario == null || usuario.Id < 1)
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
                else
                {
                    return Ok(usuario);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao consultar cadastro de usuário. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint permite realizar a consulta de todos os usuários cadastrados. Esta é uma consulta paginada
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
                var (usuarios, totalDeItens) = await _iUser.GetAll(pagination.pagina, pagination.tamanho);
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
                return StatusCode(500, $"Falha ao consultar cadastro de usuários. Erro: {ex.Message}");
            }
        }
    }
}
