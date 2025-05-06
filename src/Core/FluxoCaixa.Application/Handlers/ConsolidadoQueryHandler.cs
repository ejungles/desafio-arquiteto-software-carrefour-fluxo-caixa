using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Queries;
using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Shared.DTOs.Consolidados.Responses;
using MediatR;

public class ConsolidadoQueryHandler :
    IRequestHandler<ObterConsolidadoDiarioQuery, ConsolidadoDiarioResponse>,
    IRequestHandler<GerarRelatorioConsolidadoQuery, RelatorioConsolidadoResponse>
{
    private readonly IConsolidadoRepository _repository;
    private readonly ICacheService _cacheService;

    public ConsolidadoQueryHandler(
        IConsolidadoRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<ConsolidadoDiarioResponse> Handle(
        ObterConsolidadoDiarioQuery request,
        CancellationToken cancellationToken)
    {
        var saldoCache = await _cacheService.ObterSaldoAsync(request.Data);
        if (saldoCache != null)
            return MapearParaResponse(saldoCache);

        var saldoMongo = await _repository.GetSaldoDiarioAsync(request.Data);
        if (saldoMongo == null)
            return null;

        await _cacheService.AtualizarSaldoAsync(request.Data, saldoMongo);
        return MapearParaResponse(saldoMongo);
    }

    public async Task<RelatorioConsolidadoResponse> Handle(
        GerarRelatorioConsolidadoQuery request,
        CancellationToken cancellationToken)
    {
        var saldos = await _repository.GerarRelatorioPeriodoAsync(
            request.DataInicio,
            request.DataFim,
            request.Formato);

        var totalCreditos = saldos.Sum(s => s.TotalCreditos);
        var totalDebitos = saldos.Sum(s => s.TotalDebitos);

        return new RelatorioConsolidadoResponse
        {
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            TotalCreditos = totalCreditos,
            TotalDebitos = totalDebitos,
            SaldoPeriodo = totalCreditos - totalDebitos
        };
    }

    private ConsolidadoDiarioResponse MapearParaResponse(SaldoConsolidado saldo)
    {
        return new ConsolidadoDiarioResponse
        {
            Data = saldo.Data,
            TotalCreditos = saldo.TotalCreditos,
            TotalDebitos = saldo.TotalDebitos,
            Saldo = saldo.Saldo
        };
    }
}