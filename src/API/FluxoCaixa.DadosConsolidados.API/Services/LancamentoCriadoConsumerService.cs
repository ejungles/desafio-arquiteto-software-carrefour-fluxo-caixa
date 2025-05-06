using FluxoCaixa.Application.Handlers;
using FluxoCaixa.Shared.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FluxoCaixa.DadosConsolidados.API.Services
{
    /// <summary>
    /// Serviço hospedado para consumo de eventos de lançamento via RabbitMQ (versão compatível com RabbitMQ.Client 7.1.2)
    /// </summary>
    public class LancamentoCriadoConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LancamentoCriadoConsumerService> _logger;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel; // Atualizado: IModel para IChannel

        /// <summary>
        /// Construtor principal com injeção de dependências
        /// </summary>
        public LancamentoCriadoConsumerService(
            IServiceProvider serviceProvider,
            ILogger<LancamentoCriadoConsumerService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;

            InicializarRabbitMQAsync().GetAwaiter().GetResult(); // Inicialização assíncrona no construtor
        }

        /// <summary>
        /// Inicializa a conexão e canal com RabbitMQ usando API assíncrona (7.1.2)
        /// </summary>
        private async Task InicializarRabbitMQAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };
            
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("lancamentos.dlx", ExchangeType.Fanout, durable: true);
            await _channel.QueueDeclareAsync("lancamentos.dlq", durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync("lancamentos.dlq", "lancamentos.dlx", routingKey: string.Empty);

            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "lancamentos.dlx" }, // Exchange para mensagens falhas
                { "x-message-ttl", 30000 } // Tempo antes de mover para DLX (30 segundos)
            };

            await _channel.QueueDeclareAsync(
                queue: "lancamentos.criados",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args
            );

            _logger.LogInformation("RabbitMQ conectado (host: {Host}) e fila 'lancamentos.criados' declarada.", factory.HostName);
        }

        /// <summary>
        /// Executa a escuta contínua da fila de mensagens
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    _logger.LogInformation("Mensagem recebida: {Message}", message);

                    var evento = JsonSerializer.Deserialize<LancamentoCriadoEvent>(message);

                    if (evento == null)
                    {
                        _logger.LogWarning("Falha ao deserializar mensagem.");
                        await _channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false);
                        return;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<LancamentoCriadoEventHandler>();

                    await handler.HandleAsync(evento);

                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                    _logger.LogInformation("Mensagem processada com sucesso: {Id}", evento.LancamentoId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem recebida do RabbitMQ.");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "lancamentos.criados",
                autoAck: false,
                consumer: consumer);

            // Mantém o serviço em execução até o token de cancelamento ser acionado
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Libera os recursos de conexão e canal ao encerrar o serviço
        /// </summary>
        public override async void Dispose()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            base.Dispose();
        }
    }
}