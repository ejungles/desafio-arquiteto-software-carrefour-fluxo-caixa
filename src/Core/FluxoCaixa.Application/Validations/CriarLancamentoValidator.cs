using FluxoCaixa.Shared.DTOs.Lancamentos.Requests;
using FluentValidation;
using System;

namespace FluxoCaixa.Application.Validations
{
    /// <summary>
    /// Validador para a criação de lançamentos financeiros
    /// </summary>
    public class CriarLancamentoValidator : AbstractValidator<CriarLancamentoRequest>
    {
        /// <summary>
        /// Configura as regras de validação para o request de criação de lançamentos
        /// </summary>
        public CriarLancamentoValidator()
        {
            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage("Descrição é obrigatória")
                .MaximumLength(255)
                .WithMessage("Máximo de 255 caracteres permitidos");

            RuleFor(x => x.TipoLancamentoId)
                .GreaterThan(0)
                .WithMessage("ID do tipo de lançamento inválido")
                .LessThan(int.MaxValue)
                .WithMessage("ID do tipo de lançamento excede o limite");

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("Valor deve ser positivo")
                .WithMessage("Formato monetário inválido (ex: 12345.67)");

            RuleFor(x => x.Data)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Data não pode ser futura")
                .GreaterThanOrEqualTo(new DateTime(2000, 1, 1))
                .WithMessage("Data mínima permitida é 01/01/2000");
        }
    }
}