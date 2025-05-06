using FluxoCaixa.Application.Commands;
using FluxoCaixa.Application.Queries;
using FluxoCaixa.Shared.DTOs.Consolidados.Requests;
using FluxoCaixa.Shared.DTOs.Consolidados.Responses;
using FluxoCaixa.Shared.DTOs.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FluxoCaixa.DadosConsolidados.API.Controllers
{
    /// <summary>
    /// Controller responsável por operações de consulta de dados consolidados
    /// </summary>
    [ApiController]
    [Route("api/consolidados")]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(Policy = "RequireConsolidadoAccess")]
    public class ConsolidadosController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Construtor principal que injeta dependências necessárias
        /// </summary>
        public ConsolidadosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém o saldo consolidado para uma data específica
        /// </summary>
        /// <param name="data">Data no formato yyyy-MM-dd</param>
        /// <returns>Saldo consolidado do dia</returns>
        /// <response code="200">Retorna o saldo consolidado</response>
        /// <response code="404">Nenhum registro encontrado para a data</response>
        [HttpGet("diario/{data}")]
        [ProducesResponseType(typeof(ConsolidadoDiarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaldoDiario([FromRoute] DateTime data)
        {
            var query = new ObterConsolidadoDiarioQuery(data);
            var resultado = await _mediator.Send(query);
            return resultado != null ? Ok(resultado) : NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Mensagem = "Nenhum saldo consolidado encontrado para a data informada"
            });
        }

        /// <summary>
        /// Gera relatório consolidado para um período específico
        /// </summary>
        /// <param name="request">Parâmetros de filtro</param>
        /// <returns>Relatório consolidado no formato solicitado</returns>
        /// <response code="200">Relatório gerado com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        [HttpPost("relatorio")]
        [ProducesResponseType(typeof(RelatorioConsolidadoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GerarRelatorioPeriodo([FromBody] FiltrarConsolidadoRequest request)
        {
            var query = new GerarRelatorioConsolidadoQuery(request.DataInicio, request.DataFim, request.Formato);
            var resultado = await _mediator.Send(query);
            return Ok(resultado);
        }


        /// <summary>
        /// Endpoint responsável por reprocessar os saldos consolidados de um intervalo de datas
        /// </summary>
        /// <param name="request">Objeto contendo data de início e fim</param>
        /// <returns>NoContent se o reprocessamento for bem-sucedido</returns>
        [HttpPost("reprocessar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReprocessarSaldos([FromBody] ReprocessarConsolidadoRequest request)
        {
            // Validação manual das datas
            if (request.DataInicio > request.DataFim)
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Intervalo inválido",
                    Detail = "A data inicial não pode ser posterior à data final"
                });

            // Disparo do comando via MediatR
            var comando = new ReprocessarConsolidadoCommand(request.DataInicio, request.DataFim);
            await _mediator.Send(comando);

            return NoContent();
        }
    }
}