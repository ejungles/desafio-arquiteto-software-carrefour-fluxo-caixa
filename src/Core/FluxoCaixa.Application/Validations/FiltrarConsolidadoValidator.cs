using FluentValidation;
using FluxoCaixa.Shared.DTOs.Consolidados.Requests;

namespace FluxoCaixa.Application.Validations
{
    namespace FluxoCaixa.Application.Validations
    {
        /// <summary>
        /// Validador para filtros de relatório consolidado
        /// </summary>
        public class FiltrarConsolidadoValidator : AbstractValidator<FiltrarConsolidadoRequest>
        {
            public FiltrarConsolidadoValidator()
            {
                RuleFor(x => x.DataInicio)
                    .LessThanOrEqualTo(x => x.DataFim)
                    .WithMessage("Data inicial não pode ser posterior à data final");

                RuleFor(x => x.Formato)
                    .Must(BeValidFormat)
                    .WithMessage("Formato inválido. Valores permitidos: JSON, CSV, PDF");
            }

            private bool BeValidFormat(string formato)
            {
                return new[] { "json", "csv", "pdf" }.Contains(formato.ToLower());
            }
        }
    }

}
