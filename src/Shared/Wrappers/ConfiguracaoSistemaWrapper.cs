namespace FluxoCaixa.Shared.Wrappers
{
    /// <summary>
    /// Parâmetros de configuração do sistema.
    /// </summary>
    public class ConfiguracaoSistemaWrapper
    {
        /// <summary>
        /// Frequência de processamento automático (diária/horária).
        /// </summary>
        public string FrequenciaProcessamento { get; set; }

        /// <summary>
        /// Hora para processamento automático (formato HH:mm).
        /// </summary>
        public string HoraProcessamento { get; set; }

        /// <summary>
        /// Limite mínimo para alerta de saldo baixo.
        /// </summary>
        public decimal LimiteAlertaSaldo { get; set; }
    }
}