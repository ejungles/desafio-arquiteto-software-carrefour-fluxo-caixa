using Microsoft.EntityFrameworkCore.Storage;

namespace FluxoCaixa.Application.Interfaces
{
    /// <summary>
    /// Interface para padrão Unit of Work
    /// </summary>
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();

        Task CommitAsync();
        Task RollbackAsync();
        IDbContextTransaction CurrentTransaction { get; }
        IExecutionStrategy CreateExecutionStrategy();
    }
}
