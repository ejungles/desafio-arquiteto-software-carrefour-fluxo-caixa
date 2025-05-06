using FluxoCaixa.Domain.Entities;

namespace FluxoCaixa.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface específica para operações de lançamentos financeiros
    /// </summary>
    public interface ILancamentoRepository : IRepositoryBase<Lancamento>
    {
        /// <summary>
        /// Obtém lançamentos por data com paginação
        /// </summary>
        /// <param name="data">Data de referência para filtro</param>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Itens por página (padrão: 10)</param>
        /// <returns>Coleção de lançamentos paginados</returns>
        Task<IEnumerable<Lancamento>> GetByDateAsync(DateTime data, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Obtém lançamentos não processados para consolidação
        /// </summary>
        /// <param name="data">Data de referência para filtro</param>
        /// <returns>Coleção de lançamentos não processados</returns>
        Task<IEnumerable<Lancamento>> GetNaoProcessadosAsync(DateTime data);

        /// <summary>
        /// Atualiza o status de processamento em massa
        /// </summary>
        /// <param name="ids">Coleção de IDs dos lançamentos</param>
        /// <param name="processado">Novo status de processamento</param>
        Task BulkUpdateProcessamentoAsync(IEnumerable<long> ids, bool processado);

        /// <summary>
        /// Calcula o total de créditos para uma data específica
        /// </summary>
        /// <param name="data">Data de referência para cálculo</param>
        /// <returns>Valor total de créditos</returns>
        Task<decimal> GetTotalCreditosAsync(DateTime data);

        /// <summary>
        /// Calcula o total de débitos para uma data específica
        /// </summary>
        /// <param name="data">Data de referência para cálculo</param>
        /// <returns>Valor total de débitos</returns>
        Task<decimal> GetTotalDebitosAsync(DateTime data);

        /// <summary>
        /// Retorna todos os lançamentos financeiros de uma data específica
        /// </summary>
        /// <param name="data">Data dos lançamentos</param>
        /// <returns>Lista de lançamentos encontrados</returns>
        Task<IEnumerable<Lancamento>> ObterPorDataAsync(DateTime data);
    }
}