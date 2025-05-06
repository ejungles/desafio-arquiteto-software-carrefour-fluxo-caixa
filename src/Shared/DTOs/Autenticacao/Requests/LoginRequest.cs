using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Shared.DTOs.Autenticacao.Requests
{
    /// <summary>
    /// DTO para autenticação de usuário
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        /// <example>admin@fluxocaixa.com</example>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        /// <summary>
        /// Senha de acesso
        /// </summary>
        /// <example>SenhaSegura123!</example>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}