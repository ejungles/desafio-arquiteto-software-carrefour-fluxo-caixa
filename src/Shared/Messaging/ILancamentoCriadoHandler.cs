namespace FluxoCaixa.Shared.Messaging
{
    public interface ILancamentoCriadoHandler
    {
        Task HandleAsync(LancamentoCriadoEvent evento);
    }
}
