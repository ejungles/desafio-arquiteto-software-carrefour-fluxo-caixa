namespace FluxoCaixa.Shared.DTOs.Shared
{
    /// <summary>
    /// DTO padrão para respostas de erro
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Código do erro
        /// </summary>
        /// <example>500</example>
        public int StatusCode { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro
        /// </summary>
        /// <example>Erro ao processar a requisição</example>
        public string Mensagem { get; set; }

        /// <summary>
        /// Detalhes técnicos (opcional)
        /// </summary>
        public string Detalhes { get; set; }
    }
}