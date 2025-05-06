using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Infra.DataContext;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FluxoCaixa.Infra.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de dados consolidados
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

        /// <inheritdoc/>
        public async Task<SaldoConsolidado> GetSaldoDiarioAsync(DateTime data)
        {
            var filter = Builders<SaldoConsolidado>.Filter.Eq(s => s.Data, data.Date);
            return await _context.SaldosConsolidados.Find(filter).FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task UpsertSaldoDiarioAsync(SaldoConsolidado saldo)
        {
            var filter = Builders<SaldoConsolidado>.Filter.Eq(s => s.Data, saldo.Data);
            var options = new ReplaceOptions { IsUpsert = true };
            await _context.SaldosConsolidados.ReplaceOneAsync(filter, saldo, options);
        }

        /// <inheritdoc/>
        public async Task<bool> ExisteSaldoParaDataAsync(DateTime data)
        {
            var filter = Builders<SaldoConsolidado>.Filter.Eq(s => s.Data, data.Date);
            return await _context.SaldosConsolidados.Find(filter).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task<RelatorioConsolidado> GerarRelatorioPeriodoAsync(DateTime inicio, DateTime fim, string formato)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "Data", new BsonDocument
                        {
                            { "$gte", inicio.Date },
                            { "$lte", fim.Date }
                        }
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "TotalCreditos", new BsonDocument("$sum", "$TotalCreditos") },
                    { "TotalDebitos", new BsonDocument("$sum", "$TotalDebitos") },
                    { "SaldoTotal", new BsonDocument("$sum", "$Saldo") }
                })
            };

            var result = await _context.SaldosConsolidados
                .Aggregate<BsonDocument>(pipeline)
                .FirstOrDefaultAsync();

            return new RelatorioConsolidado
            {
                DataInicio = inicio,
                DataFim = fim,
                Formato = formato,
                Conteudo = result?.ToJson()
            };
        }
    }
}