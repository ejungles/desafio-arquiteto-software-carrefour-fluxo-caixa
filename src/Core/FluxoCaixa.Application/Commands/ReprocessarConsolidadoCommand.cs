using MediatR;

namespace FluxoCaixa.Application.Commands
{
    /// <summary>
    /// Comando que representa uma solicitação para reprocessar os saldos consolidados
    /// em um intervalo de datas específico.
    /// </summary>
    public class ReprocessarConsolidadoCommand : IRequest
    {
        /// <summary>
        /// Data inicial do intervalo de reprocessamento.
        /// </summary>
        public DateTime DataInicio { get; }

        /// <summary>
        /// Data final do intervalo de reprocessamento.
        /// </summary>
        public DateTime DataFim { get; }

        /// <summary>
        /// Construtor que inicializa o comando com as datas do intervalo.
        /// </summary>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
        public ReprocessarConsolidadoCommand(DateTime dataInicio, DateTime dataFim)
        {
            DataInicio = dataInicio.Date;
            DataFim = dataFim.Date;
        }
    }
}
