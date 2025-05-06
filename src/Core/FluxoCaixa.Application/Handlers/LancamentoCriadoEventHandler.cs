using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.Handlers
{
    /// <summary>
    /// Manipulador de eventos para processar lançamentos recebidos via RabbitMQ
    /// </summary>
    public class LancamentoCriadoEventHandler: ILancamentoCriadoHandler
    {
        private readonly IConsolidadoService _consolidadoService;
        private readonly ILogger<LancamentoCriadoEventHandler> _logger;

        /// <summary>
        /// Construtor que injeta as dependências necessárias
        /// </summary>
        /// <param name="consolidadoService">Serviço de consolidação de lançamentos</param>
        /// <param name="logger">Logger para registrar logs de execução</param>
        public LancamentoCriadoEventHandler(
            IConsolidadoService consolidadoService,
            ILogger<LancamentoCriadoEventHandler> logger)
        {
            _consolidadoService = consolidadoService;
            _logger = logger;
        }

        /// <summary>
        /// Executa o processamento do evento de lançamento
        /// </summary>
        /// <param name="evento">Evento recebido contendo os dados do lançamento</param>
        /// <returns>Task assíncrona representando a operação</returns>
        public async Task HandleAsync(LancamentoCriadoEvent evento)
        {
            _logger.LogInformation("Evento recebido: Processando lançamento ID {LancamentoId}", evento.LancamentoId);

            try
            {
                await _consolidadoService.ProcessarLancamentoAsync(evento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o evento de lançamento ID {LancamentoId}", evento.LancamentoId);
                throw;
            }
        }
    }
}
