using FluxoCaixa.Shared.DTOs.Autenticacao.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FluxoCaixa.DadosConsolidados.API.Controllers
{
    /// <summary>
    /// Controller para operações de autenticação e geração de tokens JWT
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Construtor principal com injeção de dependências
        /// </summary>
        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Autentica usuário e retorna token JWT
        /// </summary>
        /// <param name="request">Credenciais de login</param>
        /// <returns>Token JWT e dados de expiração</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validação hardcoded para exemplo (substituir por serviço real)
            if (request.Email != "admin@fluxocaixa.com" || request.Senha != "senhaSegura123!")
                return Unauthorized(new { message = "Credenciais inválidas" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Role, "Administrador"),
                new Claim("CustomClaim", "AcessoTotal")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}