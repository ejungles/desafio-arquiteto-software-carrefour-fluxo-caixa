using FluentValidation;
using FluxoCaixa.Shared.DTOs.Lancamentos.Requests;

namespace FluxoCaixa.Application.Validations
{
    public class AtualizarLancamentoValidator : AbstractValidator<AtualizarLancamentoRequest>
    {
        public AtualizarLancamentoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID inválido");

            RuleFor(x => x.NovaDescricao)
                .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.NovaDescricao))
                .WithMessage("Máximo de 255 caracteres");

            RuleFor(x => x.NovoValor)
                .GreaterThan(0).When(x => x.NovoValor.HasValue)
                .WithMessage("Valor deve ser positivo");
        }
    }
}
