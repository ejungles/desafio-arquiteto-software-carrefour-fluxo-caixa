using FluxoCaixa.Shared.Messaging;

namespace FluxoCaixa.Application.Interfaces
{
    /// <summary>
    /// Interface de contrato para serviços relacionados a saldos consolidados.
    /// </summary>
    public interface IConsolidadoService
    {
        /// <summary>
        /// Processa um lançamento individual, atualizando o saldo do dia correspondente.
        /// </summary>
        /// <param name="lancamentoCriadoEvent">Evento de criação do lançamento</param>
        Task ProcessarLancamentoAsync(LancamentoCriadoEvent lancamentoCriadoEvent);

        /// <summary>
        /// Reprocessa os saldos consolidados em um intervalo de datas.
        /// Essa operação deve reconstruir os saldos a partir dos lançamentos armazenados.
        /// </summary>
        /// <param name="dataInicio">Data inicial do período</param>
        /// <param name="dataFim">Data final do período</param>
        Task ReprocessarSaldosAsync(DateTime dataInicio, DateTime dataFim);
    }
}
