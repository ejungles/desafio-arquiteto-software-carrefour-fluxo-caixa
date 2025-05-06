namespace FluxoCaixa.Shared.Wrappers
{
    /// <summary>
    /// Representa o saldo consolidado de um dia específico.
    /// </summary>
    public class ConsolidadoDiarioWrapper
    {
        /// <summary>
        /// Data do consolidado (formato: yyyy-MM-dd).
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Soma total de créditos no período.
        /// </summary>
        public decimal TotalCreditos { get; set; }

        /// <summary>
        /// Soma total de débitos no período.
        /// </summary>
        public decimal TotalDebitos { get; set; }

        /// <summary>
        /// Saldo líquido do dia (TotalCreditos - TotalDebitos).
        /// </summary>
        public decimal Saldo { get; set; }
    }
}