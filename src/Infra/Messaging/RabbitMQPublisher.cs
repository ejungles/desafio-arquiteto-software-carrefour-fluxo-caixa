using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Shared.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FluxoCaixa.Infra.Messaging
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly RabbitMQSettings _settings;

        public RabbitMQPublisher(RabbitMQSettings settings)
        {
            _settings = settings;
            
            var factory = new ConnectionFactory {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            // Conexão assíncrona
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            
            // Canal assíncrono
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            // Configuração do exchange
            _channel.ExchangeDeclareAsync(
                exchange: _settings.LancamentosExchange,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null
            ).GetAwaiter().GetResult();
        }

        public async Task PublishAsync(LancamentoCriadoEvent message)
        {
            var properties = new BasicProperties {
                Persistent = true,
                DeliveryMode = (DeliveryModes)2 // Persistente
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            
            await _channel.BasicPublishAsync(
                exchange: _settings.LancamentosExchange,
                routingKey: "",
                mandatory: true,
                basicProperties: properties,
                body: body
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel?.IsClosed == false)
                await _channel.CloseAsync();

            GC.SuppressFinalize(this);
        }
    }
}