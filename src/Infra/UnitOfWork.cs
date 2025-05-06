using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Infra.DataContext;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Infra
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlServerDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(SqlServerDbContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém a transação atual
        /// </summary>
        public IDbContextTransaction CurrentTransaction { get; private set; }

        /// <summary>
        /// Inicia uma nova transação no banco de dados
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (CurrentTransaction != null)
            {
                _logger.LogInformation("Uma transação já está em andamento");
                return;
            }

            CurrentTransaction = await _dbContext.Database.BeginTransactionAsync();
            _logger.LogInformation($"Transação iniciada com ID: {CurrentTransaction.TransactionId}");
        }

        /// <summary>
        /// Confirma a transação em andamento
        /// </summary>
        public async Task CommitAsync()
        {
            try
            {
                if (CurrentTransaction == null)
                {
                    _logger.LogWarning("Nenhuma transação ativa para confirmar");
                    return;
                }

                await CurrentTransaction.CommitAsync();
                _logger.LogInformation($"Transação confirmada com ID: {CurrentTransaction.TransactionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao confirmar a transação: {ex.Message}");
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Cancela a transação atual
        /// </summary>
        public async Task RollbackAsync()
        {
            try
            {
                if (CurrentTransaction == null)
                {
                    _logger.LogWarning("Nenhuma transação ativa para desfazer");
                    return;
                }

                await CurrentTransaction.RollbackAsync();
                _logger.LogInformation($"Transação desfeita com ID: {CurrentTransaction.TransactionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao desfazer a transação: {ex.Message}");
                throw;
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Cria uma estratégia de execução para operações no banco de dados
        /// </summary>
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _dbContext.Database.CreateExecutionStrategy();
        }
    }
}
