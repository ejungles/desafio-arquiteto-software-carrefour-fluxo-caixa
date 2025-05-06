using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Infra.DataContext;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixa.Infra.Repositories
{
    /// <summary>
    /// Implementação do repositório de processamento
    /// </summary>
    public class ProcessamentoRepository : IProcessamentoRepository
    {
        private readonly SqlServerDbContext _context;

        /// <summary>
        /// Construtor principal que inicializa o contexto do SQL Server
        /// </summary>
        /// <param name="context">Instância configurada do contexto</param>
        /// <exception cref="ArgumentNullException">Se o contexto for nulo</exception>
        public ProcessamentoRepository(SqlServerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ProcessamentoConsolidacao> IniciarProcessamentoAsync(DateTime data)
        {
            var processamento = new ProcessamentoConsolidacao
            {
                DataProcessamento = data.Date,
                DataHoraInicio = DateTime.UtcNow,
                Status = "Iniciado"
            };

            await _context.ProcessamentoConsolidacao.AddAsync(processamento);
            await _context.SaveChangesAsync();
            return processamento;
        }

        public async Task FinalizarProcessamentoAsync(long processamentoId, string status, string mensagemErro = null)
        {
            var processamento = await _context.ProcessamentoConsolidacao.FindAsync(processamentoId);
            if (processamento != null)
            {
                processamento.DataHoraFim = DateTime.UtcNow;
                processamento.Status = status;
                processamento.MensagemErro = mensagemErro;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProcessamentoConsolidacao> GetUltimoProcessamentoAsync(DateTime data)
        {
            return await _context.ProcessamentoConsolidacao
                .Where(p => p.DataProcessamento == data.Date)
                .OrderByDescending(p => p.DataHoraInicio)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExisteProcessamentoEmAndamentoAsync(DateTime data)
        {
            return await _context.ProcessamentoConsolidacao
                .AnyAsync(p => p.DataProcessamento == data.Date &&
                             p.Status == "Em Processamento");
        }
    }
}