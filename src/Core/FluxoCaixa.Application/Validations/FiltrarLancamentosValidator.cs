using FluentValidation;
using FluxoCaixa.Shared.DTOs.Lancamentos.Requests;


namespace FluxoCaixa.Application.Validations
{
    /// <summary>
    /// Validador para filtros de consulta de lançamentos
    /// </summary>
    public class FiltrarLancamentosValidator : AbstractValidator<FiltrarLancamentosRequest>
    {
        public FiltrarLancamentosValidator()
        {
            RuleFor(x => x.DataInicio)
                .LessThanOrEqualTo(x => x.DataFim)
                .When(x => x.DataInicio.HasValue && x.DataFim.HasValue)
                .WithMessage("Data inicial não pode ser posterior à data final");

            RuleFor(x => x.TipoLancamentoId)
                .GreaterThan(0).When(x => x.TipoLancamentoId.HasValue)
                .WithMessage("ID do tipo de lançamento inválido");
        }
    }
}