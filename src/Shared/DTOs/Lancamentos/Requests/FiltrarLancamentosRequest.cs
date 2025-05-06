namespace FluxoCaixa.Shared.DTOs.Lancamentos.Requests
{
    /// <summary>
    /// Filtros para consulta de lançamentos
    /// </summary>
    public class FiltrarLancamentosRequest
    {
        /// <summary>
        /// Data inicial do período (inclusivo)
        /// </summary>
        /// <example>2024-04-01</example>
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data final do período (inclusivo)
        /// </summary>
        /// <example>2024-04-30</example>
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// ID do tipo de lançamento para filtro
        /// </summary>
        /// <example>2</example>
        public int? TipoLancamentoId { get; set; }
    }
}