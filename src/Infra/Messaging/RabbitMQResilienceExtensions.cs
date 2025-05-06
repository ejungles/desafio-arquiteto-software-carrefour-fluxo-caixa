using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace FluxoCaixa.Infra.Messaging
{
    public static class RabbitMQResilienceExtensions
    {
        public static IServiceCollection AddRabbitMQResilience(this IServiceCollection services)
        {
            services.AddTransient<IConnection>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
                var factory = new ConnectionFactory
                {
                    HostName = settings.Host,
                    UserName = settings.Username,
                    Password = settings.Password
                };

                // Política assíncrona com retentativas
                var policy = Policy
                    .Handle<BrokerUnreachableException>()
                    .WaitAndRetryAsync(
                        3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                // Execução assíncrona dentro de contexto síncrono
                return policy.ExecuteAsync(async () =>
                {
                    return await factory.CreateConnectionAsync();
                }).GetAwaiter().GetResult();
            });

            return services;
        }
    }
}