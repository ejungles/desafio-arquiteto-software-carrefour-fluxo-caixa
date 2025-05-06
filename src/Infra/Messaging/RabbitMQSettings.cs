namespace FluxoCaixa.Infra.Messaging
{
    /// <summary>
    /// Configurações para conexão com o RabbitMQ
    /// </summary>
    public class RabbitMQSettings
    {
        /// <summary>
        /// Host do servidor RabbitMQ
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Nome de usuário para autenticação
        /// </summary>
        public string Username { get; set; } = "guest";

        /// <summary>
        /// Senha para autenticação
        /// </summary>
        public string Password { get; set; } = "guest";

        /// <summary>
        /// Nome do exchange para eventos de lançamentos
        /// </summary>
        public string LancamentosExchange { get; set; } = "lancamentos.exchange";

        /// <summary>
        /// Nome da fila para processamento de consolidação
        /// </summary>
        public string ConsolidacaoQueue { get; set; } = "consolidacao.queue";
    }
}