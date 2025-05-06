using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Shared.Messaging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Services
{
    /// <summary>
    /// Serviço responsável por processar eventos de lançamentos e consolidar dados
    /// </summary>
    public sealed class ConsolidadoService : IConsolidadoService
    {
        private readonly IConsolidadoRepository _consolidadoRepository;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ConsolidadoService> _logger;

        /// <summary>
        /// Construtor principal que inicializa as dependências
        /// </summary>
        public ConsolidadoService(
            IConsolidadoRepository consolidadoRepository,
            ILancamentoRepository lancamentoRepository,
            ICacheService cacheService,
            ILogger<ConsolidadoService> logger)
        {
            _consolidadoRepository = consolidadoRepository;
            _lancamentoRepository = lancamentoRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Processa um evento de lançamento criado para atualizar o saldo consolidado
        /// </summary>
        /// <param name="lancamentoEvent">Evento contendo dados do lançamento</param>
        /// <returns>Tarefa assíncrona representando a operação</returns>
        public async Task ProcessarLancamentoAsync(LancamentoCriadoEvent lancamentoEvent)
        {
            _logger.LogInformation("Iniciando processamento do lançamento {LancamentoId}", lancamentoEvent.LancamentoId);

            try
            {
                // Obter ou criar saldo consolidado para a data
                var saldo = await _consolidadoRepository.GetSaldoDiarioAsync(lancamentoEvent.DataLancamento.Date)
                            ?? new SaldoConsolidado { Data = lancamentoEvent.DataLancamento.Date };

                // Atualizar valores
                if (lancamentoEvent.Natureza == 'C')
                {
                    saldo.TotalCreditos += lancamentoEvent.Valor;
                }
                else
                {
                    saldo.TotalDebitos += lancamentoEvent.Valor;
                }

                saldo.Saldo = saldo.TotalCreditos - saldo.TotalDebitos;

                // Persistir no MongoDB
                await _consolidadoRepository.UpsertSaldoDiarioAsync(saldo);

                // Atualizar cache
                await _cacheService.AtualizarSaldoAsync(saldo.Data, saldo);

                _logger.LogInformation("Lançamento {LancamentoId} processado com sucesso", lancamentoEvent.LancamentoId);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar lançamento {LancamentoId}", lancamentoEvent.LancamentoId);
                throw;
            }
        }

        /// <summary>
        /// Reprocessa os saldos consolidados em um intervalo de datas, recriando os saldos diários
        /// a partir dos lançamentos financeiros persistidos no banco relacional (SQL Server).
        /// </summary>
        /// <param name="dataInicio">Data inicial do intervalo a ser reprocessado</param>
        /// <param name="dataFim">Data final do intervalo a ser reprocessado</param>
        /// <returns>Task que representa a conclusão do reprocessamento</returns>
        public async Task ReprocessarSaldosAsync(DateTime dataInicio, DateTime dataFim)
        {
            // Itera por cada dia no intervalo informado
            for (var data = dataInicio.Date; data <= dataFim.Date; data = data.AddDays(1))
            {
                _logger.LogInformation("Iniciando reprocessamento para o dia {Data}", data);

                // Consulta os lançamentos da data no banco relacional (SQL Server)
                var lancamentos = await _lancamentoRepository.ObterPorDataAsync(data);

                // Verifica se há lançamentos para a data
                if (lancamentos == null || !lancamentos.Any())
                {
                    _logger.LogWarning("Nenhum lançamento encontrado para {Data}. Pulando reprocessamento.", data);
                    continue;
                }

                // Calcula totais de créditos e débitos com base na natureza dos lançamentos
                var totalCreditos = lancamentos
                    .Where(l => l.TipoLancamento.Natureza == 'C')
                    .Sum(l => l.Valor);

                var totalDebitos = lancamentos
                    .Where(l => l.TipoLancamento.Natureza == 'D')
                    .Sum(l => l.Valor);

                var saldoCalculado = totalCreditos - totalDebitos;

                // Cria ou substitui o saldo consolidado para o dia
                var saldo = new SaldoConsolidado
                {
                    Data = data,
                    TotalCreditos = totalCreditos,
                    TotalDebitos = totalDebitos,
                    Saldo = saldoCalculado
                };

                // Atualiza o MongoDB com o novo saldo consolidado
                await _consolidadoRepository.UpsertSaldoDiarioAsync(saldo);

                // Atualiza o cache Redis com o novo saldo
                await _cacheService.AtualizarSaldoAsync(data, saldo);

                _logger.LogInformation("Saldo reprocessado com sucesso para {Data}", data);
            }

            _logger.LogInformation("Reprocessamento concluído para o intervalo de {DataInicio} até {DataFim}", dataInicio, dataFim);
        }


    }
}