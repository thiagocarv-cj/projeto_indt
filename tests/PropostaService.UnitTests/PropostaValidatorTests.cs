using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.UnitTests;

public class TestesValidadorProposta
{
    [Fact]
    public void FluentValidation_PendenciaComObservacaoCurta_DeveFalhar()
    {
        var validator = new PropostaService.Application.Validators.ValidadorSolicitacaoAlterarStatusProposta();
        var result = validator.Validate(new PropostaService.Application.DTOs.SolicitacaoAlterarStatusProposta(
            StatusProposta.Pendencias, "curta"));
        result.IsValid.Should().BeFalse();
    }
}
