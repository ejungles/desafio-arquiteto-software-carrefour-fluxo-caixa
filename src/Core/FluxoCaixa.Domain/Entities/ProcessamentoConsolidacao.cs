namespace FluxoCaixa.Domain.Entities
{
    /// <summary>
    /// Registro de processamento de consolidação diária
    /// </summary>
    public class ProcessamentoConsolidacao
    {
        /// <summary>
        /// ID único do processamento
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Data alvo do processamento (dia a ser consolidado)
        /// </summary>
        public DateTime DataProcessamento { get; set; }

        /// <summary>
        /// Data e hora de início do processamento
        /// </summary>
        public DateTime DataHoraInicio { get; set; }

        /// <summary>
        /// Data e hora de conclusão do processamento
        /// </summary>
        public DateTime? DataHoraFim { get; set; }

        /// <summary>
        /// Status atual do processamento
        /// </summary>
        public string Status { get; set; } = "Em Processamento";

        /// <summary>
        /// Mensagem de erro em caso de falha
        /// </summary>
        public string MensagemErro { get; set; }

        /// <summary>
        /// Quantidade total de lançamentos processados
        /// </summary>
        public int QuantidadeLancamentos { get; set; }

        /// <summary>
        /// Valor total de créditos consolidados
        /// </summary>
        public decimal ValorTotalCreditos { get; set; }

        /// <summary>
        /// Valor total de débitos consolidados
        /// </summary>
        public decimal ValorTotalDebitos { get; set; }

        /// <summary>
        /// Saldo líquido final (Créditos - Débitos)
        /// </summary>
        public decimal SaldoConsolidado { get; set; }

        /// <summary>
        /// Duração total do processamento em milissegundos
        /// </summary>
        public long? DuracaoProcessamentoMs =>
            DataHoraFim.HasValue ?
            (long)(DataHoraFim.Value - DataHoraInicio).TotalMilliseconds :
            null;
    }
}