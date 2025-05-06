namespace FluxoCaixa.Shared.Messaging
{
    /// <summary>
    /// Evento publicado no RabbitMQ quando um novo lançamento é criado.
    /// </summary>
    public class LancamentoCriadoEvent
    {
        /// <summary>
        /// ID do lançamento gerado.
        /// </summary>
        public long LancamentoId { get; set; }

        /// <summary>
        /// Valor absoluto da transação (sempre positivo).
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Natureza da operação (C=Crédito, D=Débito).
        /// </summary>
        public char Natureza { get; set; }

        /// <summary>
        /// Data de efetivação do lançamento.
        /// </summary>
        public DateTime DataLancamento { get; set; }
    }
}
