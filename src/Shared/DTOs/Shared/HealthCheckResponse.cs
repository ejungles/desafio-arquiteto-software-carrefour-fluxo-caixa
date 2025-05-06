namespace FluxoCaixa.Shared.DTOs.Shared
{
    /// <summary>
    /// DTO com status de saúde do sistema
    /// </summary>
    public class HealthCheckResponse
    {
        /// <summary>
        /// Status geral do serviço
        /// </summary>
        /// <example>Healthy</example>
        public string Status { get; set; }

        /// <summary>
        /// Tempo total de resposta do serviço
        /// </summary>
        /// <example>150</example>
        public long TempoRespostaMs { get; set; }

        /// <summary>
        /// Detalhes das dependências
        /// </summary>
        public Dictionary<string, string> Dependencias { get; set; } = new Dictionary<string, string>();
    }
}