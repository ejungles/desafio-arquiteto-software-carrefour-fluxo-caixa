using FluxoCaixa.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace FluxoCaixa.Application.Interfaces
{
    /// <summary>
    /// Interface para serviços de cache
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Obtém o saldo consolidado de uma data específica do cache
        /// </summary>
        /// <param name="data">Data do saldo consolidado</param>
        /// <returns>Saldo consolidado armazenado em cache ou null se não existir</returns>
        Task<SaldoConsolidado> ObterSaldoAsync(DateTime data);

        /// <summary>
        /// Atualiza ou insere o saldo consolidado no cache
        /// </summary>
        /// <param name="data">Data do saldo consolidado</param>
        /// <param name="saldo">Dados do saldo consolidado</param>
        /// <returns>Tarefa assíncrona representando a operação</returns>
        Task AtualizarSaldoAsync(DateTime data, SaldoConsolidado saldo);
    }
}