using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Validations;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Shared.DTOs.Lancamentos.Requests;
using FluxoCaixa.Shared.DTOs.Lancamentos.Responses;
using FluxoCaixa.Shared.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace FluxoCaixa.Lancamentos.API.Controllers
{
    /// <summary>
    /// Controller responsável pelas operações de lançamentos financeiros
    /// </summary>
    /// <remarks>
    /// Construtor principal que inicializa as dependências
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class LancamentosController(
        ILancamentoRepository lancamentoRepository,
        IUnitOfWork unitOfWork,
        CriarLancamentoValidator validator,
        IRabbitMQPublisher rabbitMQPublisher) : ControllerBase
    {
        private readonly ILancamentoRepository _lancamentoRepository = lancamentoRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly CriarLancamentoValidator _validator = validator;
        private readonly IRabbitMQPublisher _rabbitMQPublisher = rabbitMQPublisher;

        /// <summary>
        /// Cria um novo lançamento financeiro
        /// </summary>
        /// <param name="request">Dados do lançamento a ser criado</param>
        /// <returns>O lançamento criado com seu ID</returns>
        /// <response code="201">Lançamento criado com sucesso</response>
        /// <response code="400">Dados inválidos na requisição</response>
        /// <response code="500">Erro interno no servidor</response>
        [HttpPost]
        public async Task<IActionResult> CriarLancamento([FromBody] CriarLancamentoRequest request)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var lancamento = new Lancamento
                    {
                        Descricao = request.Descricao,
                        TipoLancamentoId = request.TipoLancamentoId,
                        Valor = request.Valor,
                        DataLancamento = request.Data,
                        ProcessadoConsolidacao = false
                    };

                    await _lancamentoRepository.AddAsync(lancamento);
                    await _unitOfWork.CommitAsync();

                    _ = Task.Run(() => _rabbitMQPublisher.PublishAsync(new LancamentoCriadoEvent
                    {
                        LancamentoId = lancamento.Id,
                        Valor = lancamento.Valor,
                        Natureza = lancamento.TipoLancamento?.Natureza ?? 'C',
                        DataLancamento = lancamento.DataLancamento
                    }));

                    var response = new LancamentoResponse
                    {
                        Id = lancamento.Id,
                        Descricao = lancamento.Descricao,
                        Valor = lancamento.Valor,
                        Data = lancamento.DataLancamento,
                        TipoLancamento = lancamento.TipoLancamento != null
                            ? new TipoLancamentoResponse
                            {
                                Id = lancamento.TipoLancamento.Id,
                                Descricao = lancamento.TipoLancamento.Descricao,
                                Natureza = lancamento.TipoLancamento.Natureza
                            }
                            : null
                    };

                    return CreatedAtAction(nameof(ObterPorId), new { id = lancamento.Id }, response);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    return StatusCode(500, new { Mensagem = "Erro ao criar lançamento.", Erro = ex.Message });
                }
            });
        }


        /// <summary>
        /// Obtém um lançamento pelo seu ID
        /// </summary>
        /// <param name="id">ID do lançamento</param>
        /// <returns>Detalhes completos do lançamento</returns>
        /// <response code="200">Lançamento encontrado</response>
        /// <response code="404">Lançamento não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LancamentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] long id)
        {
            var lancamento = await _lancamentoRepository.GetByIdAsync(id);

            if (lancamento == null)
            {
                return NotFound();
            }

            var response = new LancamentoResponse
            {
                Id = lancamento.Id,
                Descricao = lancamento.Descricao,
                Valor = lancamento.Valor,
                Data = lancamento.DataLancamento,
                TipoLancamento = new TipoLancamentoResponse
                {
                    Id = lancamento.TipoLancamento.Id,
                    Descricao = lancamento.TipoLancamento.Descricao,
                    Natureza = lancamento.TipoLancamento.Natureza
                }
            };

            return Ok(response);
        }
    }
}