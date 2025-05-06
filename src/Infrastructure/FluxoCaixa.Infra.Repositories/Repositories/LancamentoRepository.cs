using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Infra.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de lançamentos financeiros
    /// </summary>
    public class LancamentoRepository : ILancamentoRepository
    {
        private readonly SqlServerDbContext _context;

        /// <summary>
        /// Construtor principal que inicializa o contexto do banco de dados
        /// </summary>
        /// <param name="context">Instância do contexto do SQL Server</param>
        /// <exception cref="ArgumentNullException">Lançada se o contexto for nulo</exception>
        public LancamentoRepository(SqlServerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public async Task AddAsync(Lancamento entity)
        {
            await _context.Lancamentos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Lancamentos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Lancamento>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Lancamentos
                .Include(l => l.TipoLancamento)
                .OrderBy(l => l.DataLancamento)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Lancamento> GetByIdAsync(long id)
        {
            return await _context.Lancamentos
                .Include(l => l.TipoLancamento)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Lancamento entity)
        {
            _context.Lancamentos.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Lancamento>> GetByDateAsync(DateTime data, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Lancamentos
                .Include(l => l.TipoLancamento)
                .Where(l => l.DataLancamento == data.Date)
                .OrderBy(l => l.DataLancamento)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Lancamento>> GetNaoProcessadosAsync(DateTime data)
        {
            return await _context.Lancamentos
                .Include(l => l.TipoLancamento)
                .Where(l => l.DataLancamento == data.Date && !l.ProcessadoConsolidacao)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task BulkUpdateProcessamentoAsync(IEnumerable<long> ids, bool processado)
        {
            await _context.Lancamentos
                .Where(l => ids.Contains(l.Id))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(l => l.ProcessadoConsolidacao, processado));

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalCreditosAsync(DateTime data)
        {
            return await _context.Lancamentos
                .Join(_context.TiposLancamento,
                    l => l.TipoLancamentoId,
                    t => t.Id,
                    (l, t) => new { Lancamento = l, Tipo = t })
                .Where(x => x.Lancamento.DataLancamento == data.Date && x.Tipo.Natureza == 'C')
                .SumAsync(x => x.Lancamento.Valor);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalDebitosAsync(DateTime data)
        {
            return await _context.Lancamentos
                .Join(_context.TiposLancamento,
                    l => l.TipoLancamentoId,
                    t => t.Id,
                    (l, t) => new { Lancamento = l, Tipo = t })
                .Where(x => x.Lancamento.DataLancamento == data.Date && x.Tipo.Natureza == 'D')
                .SumAsync(x => x.Lancamento.Valor);
        }
    }
}