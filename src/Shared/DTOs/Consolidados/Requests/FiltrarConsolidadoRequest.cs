namespace FluxoCaixa.Shared.DTOs.Consolidados.Requests
{
    /// <summary>
    /// Filtros para consulta de dados consolidados
    /// </summary>
    public class FiltrarConsolidadoRequest
    {
        /// <summary>
        /// Data inicial do período (inclusivo)
        /// </summary>
        /// <example>2024-04-01</example>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data final do período (inclusivo)
        /// </summary>
        /// <example>2024-04-30</example>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Formato de saída do relatório
        /// </summary>
        /// <example>json</example>
        public string Formato { get; set; } = "json";
    }
}