using FluentValidation;
using FluxoCaixa.Shared.DTOs.Autenticacao.Requests;

namespace FluxoCaixa.Application.Validations
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Formato de email inválido");

            RuleFor(x => x.Senha)
                .MinimumLength(8)
                .WithMessage("Senha deve ter pelo menos 8 caracteres");
        }
    }
}
