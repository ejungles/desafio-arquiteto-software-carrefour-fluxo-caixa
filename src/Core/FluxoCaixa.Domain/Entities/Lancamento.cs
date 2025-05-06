namespace FluxoCaixa.Domain.Entities
{
    /// <summary>
    /// Representa um lançamento financeiro
    /// </summary>
    public class Lancamento : EntityBase
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public int TipoLancamentoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataLancamento { get; set; }
        public bool ProcessadoConsolidacao { get; set; }
        public virtual TipoLancamento TipoLancamento { get; set; }
        public DateTime? DataHoraAlteracao { get; set; }
        public string UsuarioCriacao { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string Observacao { get; set; }
    }
}
