namespace FluxoCaixa.Shared.DTOs.Consolidados.Responses
{
    /// <summary>
    /// DTO com o saldo consolidado diário
    /// </summary>
    public class ConsolidadoDiarioResponse
    {
        /// <summary>
        /// Data de referência do consolidado
        /// </summary>
        /// <example>2024-04-30</example>
        public DateTime Data { get; set; }

        /// <summary>
        /// Total de créditos no período
        /// </summary>
        /// <example>1500.00</example>
        public decimal TotalCreditos { get; set; }

        /// <summary>
        /// Total de débitos no período
        /// </summary>
        /// <example>750.50</example>
        public decimal TotalDebitos { get; set; }

        /// <summary>
        /// Saldo líquido do dia
        /// </summary>
        /// <example>749.50</example>
        public decimal Saldo { get; set; }
    }
}