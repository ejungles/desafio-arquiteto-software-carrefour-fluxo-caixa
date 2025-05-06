using FluxoCaixa.Shared.Messaging;

namespace FluxoCaixa.Application.Interfaces
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync(LancamentoCriadoEvent message);
    }
}
