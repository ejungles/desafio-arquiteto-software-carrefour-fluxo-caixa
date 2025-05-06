namespace FluxoCaixa.Shared.DTOs.Lancamentos.Responses
{
    /// <summary>
    /// DTO com informações de um tipo de lançamento
    /// </summary>
    public class TipoLancamentoResponse
    {
        /// <summary>
        /// ID do tipo de lançamento
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Descrição do tipo
        /// </summary>
        /// <example>Venda</example>
        public string Descricao { get; set; }

        /// <summary>
        /// Natureza da operação (C=Crédito, D=Débito)
        /// </summary>
        /// <example>C</example>
        public char Natureza { get; set; }
    }
}