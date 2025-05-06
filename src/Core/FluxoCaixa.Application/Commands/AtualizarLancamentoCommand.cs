using FluxoCaixa.Shared.DTOs.Lancamentos.Responses;
using MediatR;

namespace FluxoCaixa.Application.Commands
{
    /// <summary>
    /// Comando que representa a solicitação para atualizar um lançamento financeiro existente
    /// </summary>
    public class AtualizarLancamentoCommand : IRequest<LancamentoResponse>
    {
        /// <summary>
        /// Identificador único do lançamento a ser atualizado
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nova descrição do lançamento (opcional)
        /// </summary>
        public string NovaDescricao { get; set; }

        /// <summary>
        /// Novo valor do lançamento (opcional)
        /// </summary>
        public decimal? NovoValor { get; set; }

        /// <summary>
        /// Nova data do lançamento (opcional)
        /// </summary>
        public DateTime? NovaData { get; set; }

        /// <summary>
        /// Construtor padrão do comando
        /// </summary>
        public AtualizarLancamentoCommand(long id, string novaDescricao, decimal novoValor, DateTime novaData)
        {
            Id = id;
            NovaDescricao = novaDescricao;
            NovoValor = novoValor;
            NovaData = novaData;
        }
    }
}
