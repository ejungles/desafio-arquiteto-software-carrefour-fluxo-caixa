namespace FluxoCaixa.Domain.Entities
{
    /// <summary>
    /// Tipo de lançamento (Crédito/Débito)
    /// </summary>
    public class TipoLancamento : EntityBase
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public char Natureza { get; set; } // 'C' ou 'D'
        public bool Ativo { get; set; } = true;
    }
}
