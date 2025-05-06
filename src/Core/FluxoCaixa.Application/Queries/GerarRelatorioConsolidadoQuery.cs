using FluxoCaixa.Shared.DTOs.Consolidados.Responses;
using MediatR;

namespace FluxoCaixa.Application.Queries
{
    /// <summary>
    /// Query para geração de relatórios consolidados
    /// </summary>
    public class GerarRelatorioConsolidadoQuery : IRequest<RelatorioConsolidadoResponse>
    {
        /// <summary>
        /// Data inicial do período
        /// </summary>
        public DateTime DataInicio { get; }

        /// <summary>
        /// Data final do período
        /// </summary>
        public DateTime DataFim { get; }

        /// <summary>
        /// Formato de saída do relatório
        /// </summary>
        public string Formato { get; }

        /// <summary>
        /// Construtor que inicializa os parâmetros da query
        /// </summary>
        public GerarRelatorioConsolidadoQuery(DateTime dataInicio, DateTime dataFim, string formato)
        {
            DataInicio = dataInicio.Date;
            DataFim = dataFim.Date;
            Formato = formato.ToUpper();
        }
    }
}