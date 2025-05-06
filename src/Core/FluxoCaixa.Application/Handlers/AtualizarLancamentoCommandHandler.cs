using AutoMapper;
using FluxoCaixa.Application.Commands;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Shared.DTOs.Lancamentos.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FluxoCaixa.Application.Handlers
{
    /// <summary>
    /// Handler responsável por processar o comando de atualização de lançamento
    /// </summary>
    public class AtualizarLancamentoCommandHandler : IRequestHandler<AtualizarLancamentoCommand, LancamentoResponse>
    {
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AtualizarLancamentoCommandHandler> _logger;

        /// <summary>
        /// Construtor principal com injeção de dependências
        /// </summary>
        public AtualizarLancamentoCommandHandler(
            ILancamentoRepository lancamentoRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AtualizarLancamentoCommandHandler> logger)
        {
            _lancamentoRepository = lancamentoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Manipula a execução do comando de atualização de lançamento
        /// </summary>
        /// <param name="request">Dados enviados para atualização</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Resposta com os dados atualizados</returns>
        public async Task<LancamentoResponse> Handle(AtualizarLancamentoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando atualização do lançamento ID {Id}", request.Id);

            // Recupera o lançamento existente no banco de dados
            var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id);
            if (lancamento == null)
            {
                _logger.LogWarning("Lançamento ID {Id} não encontrado", request.Id);
                return null;
            }

            // Aplica as atualizações apenas nos campos informados
            if (!string.IsNullOrEmpty(request.NovaDescricao))
                lancamento.Descricao = request.NovaDescricao;

            if (request.NovoValor.HasValue)
                lancamento.Valor = request.NovoValor.Value;

            if (request.NovaData.HasValue)
                lancamento.DataLancamento = request.NovaData.Value;

            // Persiste as alterações
            await _unitOfWork.BeginTransactionAsync();
            await _lancamentoRepository.UpdateAsync(lancamento);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Lançamento ID {Id} atualizado com sucesso", request.Id);

            // Retorna os dados atualizados
            return _mapper.Map<LancamentoResponse>(lancamento);
        }
    }
}
