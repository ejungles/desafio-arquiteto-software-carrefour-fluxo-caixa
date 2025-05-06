namespace FluxoCaixa.Shared.DTOs.Lancamentos.Responses
{
    /// <summary>
    /// DTO com dados completos de um lançamento financeiro
    /// </summary>
    public class LancamentoResponse
    {
        /// <summary>
        /// ID único do lançamento
        /// </summary>
        /// <example>12345</example>
        public long Id { get; set; }

        /// <summary>
        /// Descrição detalhada
        /// </summary>
        /// <example>Pagamento de fornecedor</example>
        public string Descricao { get; set; }

        /// <summary>
        /// Valor absoluto da transação
        /// </summary>
        /// <example>250.75</example>
        public decimal Valor { get; set; }

        /// <summary>
        /// Data efetiva do lançamento
        /// </summary>
        /// <example>2024-04-30</example>
        public DateTime Data { get; set; }

        /// <summary>
        /// Tipo de lançamento associado
        /// </summary>
        public TipoLancamentoResponse TipoLancamento { get; set; }
    }
}