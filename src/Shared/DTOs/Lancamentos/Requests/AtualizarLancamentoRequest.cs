using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Shared.DTOs.Lancamentos.Requests
{
    /// <summary>
    /// DTO para atualização de um lançamento existente
    /// </summary>
    public class AtualizarLancamentoRequest
    {
        /// <summary>
        /// ID do lançamento a ser atualizado
        /// </summary>
        /// <example>123</example>
        [Required(ErrorMessage = "ID é obrigatório")]
        public long Id { get; set; }

        /// <summary>
        /// Nova descrição do lançamento
        /// </summary>
        /// <example>Venda atualizada do produto Y</example>
        [StringLength(255, ErrorMessage = "Máximo de {1} caracteres")]
        public string NovaDescricao { get; set; }

        /// <summary>
        /// Novo valor da transação
        /// </summary>
        /// <example>299.90</example>
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal? NovoValor { get; set; }
    }
}