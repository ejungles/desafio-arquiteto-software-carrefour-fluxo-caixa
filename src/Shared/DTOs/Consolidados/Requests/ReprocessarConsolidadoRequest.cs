namespace FluxoCaixa.Shared.DTOs.Consolidados.Requests
{
    /// <summary>
    /// Objeto de transferência (DTO) contendo os parâmetros de entrada
    /// para reprocessamento de saldos consolidados.
    /// </summary>
    public class ReprocessarConsolidadoRequest
    {
        /// <summary>
        /// Data inicial do período a ser reprocessado.
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data final do período a ser reprocessado.
        /// </summary>
        public DateTime DataFim { get; set; }
    }
}
