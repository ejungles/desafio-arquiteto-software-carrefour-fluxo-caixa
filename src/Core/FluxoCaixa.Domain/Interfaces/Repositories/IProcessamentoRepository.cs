using FluxoCaixa.Domain.Entities;

namespace FluxoCaixa.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface para operações de repositório de processamento
    /// </summary>
    public interface IProcessamentoRepository
    {
        /// <summary>
        /// Inicia um novo processo de consolidação
        /// </summary>
        /// <param name="data">Data de referência para consolidação</param>
        /// <returns>Processamento iniciado</returns>
        Task<ProcessamentoConsolidacao> IniciarProcessamentoAsync(DateTime data);

        /// <summary>
        /// Finaliza um processo de consolidação
        /// </summary>
        /// <param name="processamentoId">ID do processamento</param>
        /// <param name="status">Status final (Concluído/Erro)</param>
        /// <param name="mensagemErro">Mensagem de erro em caso de falha</param>
        Task FinalizarProcessamentoAsync(long processamentoId, string status, string mensagemErro = null);

        /// <summary>
        /// Obtém o último processamento para uma data específica
        /// </summary>
        /// <param name="data">Data de referência</param>
        /// <returns>Último processamento registrado</returns>
        Task<ProcessamentoConsolidacao> GetUltimoProcessamentoAsync(DateTime data);

        /// <summary>
        /// Verifica se existe processamento em andamento para uma data
        /// </summary>
        /// <param name="data">Data de verificação</param>
        /// <returns>True se existir processamento pendente</returns>
        Task<bool> ExisteProcessamentoEmAndamentoAsync(DateTime data);
    }
}