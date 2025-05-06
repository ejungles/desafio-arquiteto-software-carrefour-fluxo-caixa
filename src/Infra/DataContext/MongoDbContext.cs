using FluxoCaixa.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FluxoCaixa.Infra.DataContext
{
    /// <summary>
    /// Contexto de banco de dados para MongoDB
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Construtor principal que configura a conexão
        /// </summary>
        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:MongoDB"];

            // Extrai o nome do banco de dados da connection string
            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            EnsureIndexes();
        }


        /// <summary>
        /// Coleção de saldos consolidados diários
        /// </summary>
        public IMongoCollection<SaldoConsolidado> SaldosConsolidados =>
            _database.GetCollection<SaldoConsolidado>("saldosConsolidados");

        /// <summary>
        /// Coleção de relatórios gerados
        /// </summary>
        public IMongoCollection<RelatorioConsolidado> Relatorios =>
            _database.GetCollection<RelatorioConsolidado>("relatorios");

        /// <summary>
        /// Configuração de índices para otimização de consultas
        /// </summary>
        private void EnsureIndexes()
        {
            // Índice para Data em SaldoConsolidado
            SaldosConsolidados.Indexes.CreateOne(
                new CreateIndexModel<SaldoConsolidado>(
                    Builders<SaldoConsolidado>.IndexKeys.Ascending(s => s.Data),
                    new CreateIndexOptions { Unique = true }
                ));

            // Índice para DataInicio/DataFim em Relatorios
            Relatorios.Indexes.CreateOne(
                new CreateIndexModel<RelatorioConsolidado>(
                    Builders<RelatorioConsolidado>.IndexKeys
                        .Ascending(r => r.DataInicio)
                        .Ascending(r => r.DataFim)
                ));
        }
    }
}