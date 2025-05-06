using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Shared.DTOs.Lancamentos.Requests
{
    /// <summary>
    /// DTO para criação de um novo lançamento financeiro
    /// </summary>
    public class CriarLancamentoRequest
    {
        /// <summary>
        /// Descrição detalhada do lançamento
        /// </summary>
        /// <example>Venda de produto X</example>
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(255, ErrorMessage = "Máximo de {1} caracteres")]
        public string Descricao { get; set; }

        /// <summary>
        /// ID do tipo de lançamento (crédito/débito)
        /// </summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Tipo de lançamento inválido")]
        public int TipoLancamentoId { get; set; }

        /// <summary>
        /// Valor monetário da transação (positivo para créditos)
        /// </summary>
        /// <example>150.50</example>
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Data efetiva do lançamento
        /// </summary>
        /// <example>2024-04-30</example>
        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Data { get; set; }
    }
}