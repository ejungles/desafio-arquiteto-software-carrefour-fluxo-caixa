namespace FluxoCaixa.Shared.Wrappers
{
    /// <summary>
    /// Filtros para geração de relatórios por período.
    /// </summary>
    public class RelatorioPeriodoRequest
    {
        /// <summary>
        /// Data inicial do período (inclusivo).
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data final do período (inclusivo).
        /// </summary>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Formato de saída do relatório (JSON/CSV/PDF).
        /// </summary>
        public string Formato { get; set; }
    }
}