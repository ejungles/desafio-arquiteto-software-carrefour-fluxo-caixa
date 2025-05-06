using FluxoCaixa.Shared.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FluxoCaixa.Infra.Messaging
{
    public class RabbitMQConsumer : BackgroundService, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly RabbitMQSettings _settings;
        private readonly IServiceProvider _services;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private string _consumerTag;

        public RabbitMQConsumer(
            IOptions<RabbitMQSettings> settings,
            IServiceProvider services,
            ILogger<RabbitMQConsumer> logger)
        {
            _settings = settings.Value;
            _services = services;
            _logger = logger;
        }

        private async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await ConfigureInfrastructureAsync();
        }

        private async Task ConfigureInfrastructureAsync()
        {
            // Exchange principal
            await _channel.ExchangeDeclareAsync(
                exchange: _settings.LancamentosExchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false);

            // Configuração da fila com DLX
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", $"{_settings.LancamentosExchange}_deadletter" },
                { "x-message-ttl", 30000 }
            };

            await _channel.QueueDeclareAsync(
                queue: _settings.ConsolidacaoQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);

            await _channel.QueueBindAsync(
                queue: _settings.ConsolidacaoQueue,
                exchange: _settings.LancamentosExchange,
                routingKey: "");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    await ProcessMessageAsync(ea);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao processar mensagem {DeliveryTag}", ea.DeliveryTag);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            // Configuração de QoS para controle de concorrência
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 100, // Número de mensagens processadas simultaneamente
                global: false);

            _consumerTag = await _channel.BasicConsumeAsync(
                queue: _settings.ConsolidacaoQueue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("Consumer iniciado com tag: {ConsumerTag}", _consumerTag);
        }

        private async Task ProcessMessageAsync(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var evento = JsonSerializer.Deserialize<LancamentoCriadoEvent>(message);

            using var scope = _services.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ILancamentoCriadoHandler>();
            await handler.HandleAsync(evento);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_channel?.IsOpen == true && !string.IsNullOrEmpty(_consumerTag))
                {
                    await _channel.BasicCancelAsync(_consumerTag);
                    await _channel.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar canal");
            }

            try
            {
                if (_connection?.IsOpen == true)
                {
                    await _connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar conexão");
            }

            _channel?.Dispose();
            _connection?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}