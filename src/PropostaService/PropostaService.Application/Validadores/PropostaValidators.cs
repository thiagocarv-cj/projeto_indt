using FluentValidation;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Enums;

namespace PropostaService.Application.Validadores;

public class ValidadorSolicitacaoCriarProposta : AbstractValidator<SolicitacaoCriarProposta>
{
    public ValidadorSolicitacaoCriarProposta()
    {
        RuleFor(x => x.NomeSegurado).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cpf).NotEmpty().MaximumLength(14);
        RuleFor(x => x.ValorCobertura).GreaterThan(0);
    }
}

public class ValidadorSolicitacaoAlterarStatusProposta : AbstractValidator<SolicitacaoAlterarStatusProposta>
{
    public ValidadorSolicitacaoAlterarStatusProposta()
    {
        RuleFor(x => x.Status).IsInEnum();

        When(x => x.Status == StatusProposta.Pendencias, () =>
        {
            RuleFor(x => x.Observacao)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(500);
        });

        When(x => x.Status == StatusProposta.Rejeitada && !string.IsNullOrWhiteSpace(x.Observacao), () =>
        {
            RuleFor(x => x.Observacao).MaximumLength(500);
        });
    }
}
