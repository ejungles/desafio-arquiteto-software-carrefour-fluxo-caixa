using FluxoCaixa.Shared.DTOs.Consolidados.Responses;
using MediatR;

namespace FluxoCaixa.Application.Queries
{
    /// <summary>
    /// Query para obtenção do saldo consolidado diário
    /// </summary>
    public class ObterConsolidadoDiarioQuery : IRequest<ConsolidadoDiarioResponse>
    {
        /// <summary>
        /// Data de referência para a consulta
        /// </summary>
        public DateTime Data { get; }

        /// <summary>
        /// Construtor que inicializa a query com a data necessária
        /// </summary>
        public ObterConsolidadoDiarioQuery(DateTime data)
        {
            Data = data.Date;
        }
    }
}