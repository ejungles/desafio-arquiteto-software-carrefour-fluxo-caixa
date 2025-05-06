using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Infra.DataContext;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FluxoCaixa.Infra.Repositories
{
    /// <summary>
    /// Implementação do repositório de dados consolidados
    /// </summary>
    public class ConsolidadoRepository : IConsolidadoRepository
    {
        private readonly MongoDbContext _context;

        /// <summary>
        /// Construtor principal que inicializa o contexto do MongoDB
        /// </summary>
        /// <param name="context">Instância configurada do contexto</param>
        /// <exception cref="ArgumentNullException">Se o contexto for nulo</exception>
        public ConsolidadoRepository(MongoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SaldoConsolidado> GetSaldoDiarioAsync(DateTime data)
        {
            var filter = Builders<SaldoConsolidado>.Filter.Eq(s => s.Data, data.Date);
            return await _context.SaldosConsolidados.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpsertSaldoDiarioAsync(SaldoConsolidado saldo)
        {
            var filter = Builders<SaldoConsolidado>.Filter.Eq(s => s.Data, saldo.Data);
            var options = new ReplaceOptions { IsUpsert = true };
            await _context.SaldosConsolidados.ReplaceOneAsync(filter, saldo, options);
        }

        public async Task<bool> ExisteSaldoParaDataAsync(DateTime data)
        {
            var filter = Builders<SaldoConsolidado>.Filter.Eq(s => s.Data, data.Date);
            return await _context.SaldosConsolidados.Find(filter).AnyAsync();
        }

        public async Task<List<RelatorioConsolidado>> GerarRelatorioPeriodoAsync(DateTime inicio, DateTime fim, string formato)
        {
            // Filtra os saldos consolidados dentro do período especificado
            var saldos = await _context.SaldosConsolidados
                .Find(s => s.Data >= inicio.Date && s.Data <= fim.Date)
                .ToListAsync();

            // Converte os saldos para RelatorioConsolidado conforme o formato
            var relatorios = saldos.Select(saldo => new RelatorioConsolidado
            {
                DataInicio = saldo.Data, // Usando a data do saldo como DataInicio
                DataFim = saldo.Data,    // e DataFim (pois é diário)
                Formato = formato,
                Conteudo = new
                {
                    Data = saldo.Data,
                    TotalCreditos = saldo.TotalCreditos,
                    TotalDebitos = saldo.TotalDebitos,
                    Saldo = saldo.Saldo
                }.ToJson() // Serializa para JSON
            }).ToList();

            return relatorios;
        }
    }
}