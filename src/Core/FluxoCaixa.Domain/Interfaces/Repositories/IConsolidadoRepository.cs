using FluxoCaixa.Domain.Entities;

namespace FluxoCaixa.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface para operações de repositório de dados consolidados
    /// </summary>
    public interface IConsolidadoRepository
    {
        /// <summary>
        /// Obtém o saldo consolidado para uma data específica
        /// </summary>
        /// <param name="data">Data de referência (formato: yyyy-MM-dd)</param>
        /// <returns>Dados consolidados ou null se não existir</returns>
        Task<SaldoConsolidado> GetSaldoDiarioAsync(DateTime data);

        /// <summary>
        /// Atualiza ou insere um novo saldo consolidado
        /// </summary>
        /// <param name="saldo">Dados consolidados a serem persistidos</param>
        Task UpsertSaldoDiarioAsync(SaldoConsolidado saldo);

        /// <summary>
        /// Verifica se existe saldo consolidado para uma data específica
        /// </summary>
        /// <param name="data">Data de verificação</param>
        /// <returns>True se existir registro para a data</returns>
        Task<bool> ExisteSaldoParaDataAsync(DateTime data);

        /// <summary>
        /// Gera relatório consolidado para um período específico
        /// </summary>
        /// <param name="inicio">Data inicial do período</param>
        /// <param name="fim">Data final do período</param>
        /// <param name="formato">Formato de saída (JSON/CSV/PDF)</param>
        /// <returns>Relatório consolidado formatado</returns>
        Task<List<RelatorioConsolidado>> GerarRelatorioPeriodoAsync(DateTime inicio, DateTime fim, string formato);
    }
}
