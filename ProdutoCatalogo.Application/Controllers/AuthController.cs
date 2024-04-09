using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdutoCatalogo.Domain.DTOs.Request;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Infra.Configurations.Headers;
using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;
using ProdutoCatalogo.Domain.Entities.Token;
using ProdutoCatalogo.Domain.Interfaces.Repositories;

namespace ProdutoCatalogo.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(HeaderValidationFilter))]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwt;
        private readonly IAuth _iAuth;
        public AuthController(IJwtService jwtService, IAuth iAuth)
        {
            _jwt = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _iAuth = iAuth ?? throw new ArgumentNullException(nameof(iAuth));
        }

        private TokenAccess? ResponseRequest(int id, string nick, string email, string permission)
        {
            var claimsToken = _jwt.SetClaim(id, nick, email, permission);
            return _jwt.Generate(claimsToken);
        }

        /// <summary>
        /// Este endpoint permite gerar o token de acesso aos demais endpoints.
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
        ///     "id": 1,
        ///     "apelido": "string",
        ///     "email": "user@example.com",
        ///     "idPermissao": 1,
        ///     "permissao": "string",
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp,
                                                    [FromBody] AuthenticationLogin model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _iAuth.Login(model);
                if (user != null)
                {
                    var tokenAccess = ResponseRequest(user.Id, user.Apelido, user.Email, user.Permissao);
                    if (tokenAccess == null)
                    {
                        return StatusCode(500, ValidationMessages.Token.NotGenerate);
                    }

                    if (string.IsNullOrEmpty(tokenAccess.Token))
                    {
                        return StatusCode(500, ValidationMessages.Token.NotGenerate);
                    }

                    return Ok(new { usuario = user, tokenAcesso = tokenAccess });
                }
                else
                {
                    return NotFound(ValidationMessages.EmptyReturn);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao gerar token de acesso. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Este endpoint é responsável por atualizar o token de acesso.
        /// </summary>
        /// <response code="200">OK: Foi gerado um novo token de acesso válido.
        /// <pre>
        /// {
        ///   "tokenAcesso": {
        ///     "token": "string",
        ///     "expiration": "1900-01-01T00:00:00",
        ///     "issuedAt": "1900-01-01T00:00:00"
        ///   }
        /// }
        /// </pre>
        /// </response>
        /// <response code="400">Bad Request: Os dados fornecidos são inválidos ou incompletos.</response>
        /// <response code="500">Internal Server Error: Não foi possível concluir a solicitação por alguma falha interna.</response>
        [AllowAnonymous]
        [HttpPut("renovar-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromHeader(Name = "x-request-timestamp")][Required] DateTime headerTimestamp)
        {
            string headerAuthorization = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrEmpty(headerAuthorization))
            {
                return BadRequest(ValidationMessages.Header.Authorization.Missing);
            }

            try
            {
                var tokenAccess = _jwt.Refresh(headerAuthorization);
                if (tokenAccess == null)
                {
                    return StatusCode(500, ValidationMessages.Token.NotGenerate);
                }

                if (string.IsNullOrEmpty(tokenAccess.Token))
                {
                    return StatusCode(500, ValidationMessages.Token.NotGenerate);
                }

                return Ok(new { tokenAcesso = tokenAccess });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Falha ao renovar token de acesso. Erro: {ex.Message}");
            }
        }
    }
}
