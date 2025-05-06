namespace FluxoCaixa.Shared.Wrappers
{
    /// <summary>
    /// Representa um lançamento financeiro (débito ou crédito) para operações de entrada/saída.
    /// </summary>
    public class LancamentoWrapper
    {
        /// <summary>
        /// Identificador único do lançamento (gerado automaticamente).
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Descrição detalhada do lançamento (máximo 255 caracteres).
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// ID do tipo de lançamento associado (referência à tabela TiposLancamento).
        /// </summary>
        public int TipoLancamentoId { get; set; }

        /// <summary>
        /// Valor monetário da transação (positivo para créditos, negativo para débitos).
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Data efetiva do lançamento (formato: yyyy-MM-dd).
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Indica se o lançamento foi processado para consolidação.
        /// </summary>
        public bool ProcessadoConsolidacao { get; set; }
    }
}