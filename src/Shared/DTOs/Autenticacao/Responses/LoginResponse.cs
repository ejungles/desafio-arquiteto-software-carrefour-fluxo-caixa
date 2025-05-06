namespace FluxoCaixa.Shared.DTOs.Autenticacao.Responses
{
    /// <summary>
    /// DTO com resposta de autenticação bem-sucedida
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT para autenticação
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string Token { get; set; }

        /// <summary>
        /// Data de expiração do token
        /// </summary>
        /// <example>2024-05-01T12:00:00</example>
        public DateTime Expiracao { get; set; }
    }
}