namespace FluxoCaixa.Shared.DTOs.Consolidados.Responses
{
    /// <summary>
    /// DTO com dados de um relatório consolidado
    /// </summary>
    public class RelatorioConsolidadoResponse
    {
        /// <summary>
        /// Período inicial do relatório
        /// </summary>
        /// <example>2024-04-01</example>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Período final do relatório
        /// </summary>
        /// <example>2024-04-30</example>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Total de créditos no período
        /// </summary>
        /// <example>15000.00</example>
        public decimal TotalCreditos { get; set; }

        /// <summary>
        /// Total de débitos no período
        /// </summary>
        /// <example>7500.50</example>
        public decimal TotalDebitos { get; set; }

        /// <summary>
        /// Saldo líquido do período
        /// </summary>
        /// <example>7499.50</example>
        public decimal SaldoPeriodo { get; set; }
    }
}