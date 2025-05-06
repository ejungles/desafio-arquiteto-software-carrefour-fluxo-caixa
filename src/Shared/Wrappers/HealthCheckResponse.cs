namespace FluxoCaixa.Shared.Wrappers
{
    /// <summary>
    /// Status de saúde dos serviços e dependências.
    /// </summary>
    public class HealthCheckResponse
    {
        /// <summary>
        /// Status geral do serviço (Healthy/Unhealthy).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Tempo total de resposta em milissegundos.
        /// </summary>
        public long TempoResposta { get; set; }

        /// <summary>
        /// Detalhes das dependências (ex: Banco de dados, RabbitMQ).
        /// </summary>
        public Dictionary<string, string> Dependencias { get; set; }
    }
}