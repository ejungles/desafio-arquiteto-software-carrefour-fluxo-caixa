using System.ComponentModel.DataAnnotations;

namespace FluxoCaixa.Shared.DTOs.Shared
{
    /// <summary>
    /// Parâmetros de paginação para consultas
    /// </summary>
    public class PaginacaoRequest
    {
        /// <summary>
        /// Número da página (inicia em 1)
        /// </summary>
        /// <example>1</example>
        public int Pagina { get; set; } = 1;

        /// <summary>
        /// Itens por página
        /// </summary>
        /// <example>10</example>
        [Range(1, 100, ErrorMessage = "O tamanho da página deve ser entre 1 e 100")]
        public int TamanhoPagina { get; set; } = 10;
    }
}