using FluxoCaixa.Application.Commands;
using FluxoCaixa.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.Handlers
{
    /// <summary>
    /// Manipulador responsável por executar o comando de reprocessamento dos saldos consolidados
    /// </summary>
    public class ReprocessarConsolidadoCommandHandler : IRequestHandler<ReprocessarConsolidadoCommand>
    {
        /// <summary>
        /// Serviço responsável por orquestrar o reprocessamento dos saldos consolidados
        /// </summary>
        private readonly IConsolidadoService _consolidadoService;

        /// <summary>
        /// Logger para registrar informações e erros durante o processo de reprocessamento
        /// </summary>
        private readonly ILogger<ReprocessarConsolidadoCommandHandler> _logger;

        /// <summary>
        /// Construtor da classe que recebe as dependências via injeção
        /// </summary>
        /// <param name="consolidadoService">Serviço de domínio para consolidação de saldos</param>
        /// <param name="logger">Logger para rastreamento de execução</param>
        public ReprocessarConsolidadoCommandHandler(
            IConsolidadoService consolidadoService,
            ILogger<ReprocessarConsolidadoCommandHandler> logger)
        {
            _consolidadoService = consolidadoService;
            _logger = logger;
        }

        /// <summary>
        /// Método que executa o reprocessamento dos saldos consolidados no intervalo solicitado
        /// </summary>
        /// <param name="request">Comando contendo as datas de início e fim</param>
        /// <param name="cancellationToken">Token de cancelamento da operação</param>
        /// <returns>Objeto do tipo Unit indicando que a operação foi concluída</returns>
        public async Task<Unit> Handle(ReprocessarConsolidadoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando reprocessamento de saldos de {DataInicio} até {DataFim}", request.DataInicio, request.DataFim);

            await _consolidadoService.ReprocessarSaldosAsync(request.DataInicio, request.DataFim);

            _logger.LogInformation("Reprocessamento concluído com sucesso.");

            return Unit.Value;
        }

        /// <summary>
        /// Implementação explícita da interface IRequestHandler para manipular o comando ReprocessarConsolidadoCommand.
        /// Redireciona a chamada para o método Handle privado da classe.
        /// </summary>
        /// <param name="request">Objeto contendo as datas de início e fim do reprocessamento.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Tarefa representando a operação assíncrona.</returns>
        Task IRequestHandler<ReprocessarConsolidadoCommand>.Handle(ReprocessarConsolidadoCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }
}
