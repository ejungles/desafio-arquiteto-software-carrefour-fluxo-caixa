using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace FluxoCaixa.Infra.Caching
{
    /// <summary>
    /// Implementação do serviço de cache usando Redis
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDatabase;

        /// <summary>
        /// Construtor principal que inicializa a conexão com o Redis
        /// </summary>
        /// <param name="redisConnection">Conexão multiplexada com o Redis</param>
        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase();
        }

        /// <inheritdoc/>
        public async Task<SaldoConsolidado> ObterSaldoAsync(DateTime data)
        {
            var key = $"saldo:{data:yyyy-MM-dd}";
            var cachedData = await _redisDatabase.StringGetAsync(key);
            return cachedData.HasValue ?
                JsonSerializer.Deserialize<SaldoConsolidado>(cachedData) :
                null;
        }

        /// <inheritdoc/>
        public async Task AtualizarSaldoAsync(DateTime data, SaldoConsolidado saldo)
        {
            var key = $"saldo:{data:yyyy-MM-dd}";
            var serializedData = JsonSerializer.Serialize(saldo);
            await _redisDatabase.StringSetAsync(key, serializedData, TimeSpan.FromMinutes(30));
        }
    }
}