using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Excecoes;

namespace PropostaService.UnitTests;

public class TestesDominioProposta
{
    [Fact]
    public void Criar_DeveIniciarEmAnalise()
    {
        var proposta = Proposta.Criar("João", "12345678901", 10000m);
        proposta.Status.Should().Be(StatusProposta.EmAnalise);
        proposta.Observacao.Should().BeNull();
    }

    [Fact]
    public void MarcarComoPendente_SemObservacao_DeveLancarExcecao()
    {
        var proposta = Proposta.Criar("João", "12345678901", 10000m);
        var act = () => proposta.AlterarStatus(StatusProposta.Pendencias, null);
        act.Should().Throw<ExcecaoDominio>().WithMessage("*Observação*");
    }

    [Fact]
    public void Aprovar_DeveLimparObservacao()
    {
        var proposta = Proposta.Criar("João", "12345678901", 10000m);
        proposta.AlterarStatus(StatusProposta.Pendencias, "Documento ilegível para leitura");
        proposta.AlterarStatus(StatusProposta.Aprovada, null);
        proposta.Status.Should().Be(StatusProposta.Aprovada);
        proposta.Observacao.Should().BeNull();
    }

    [Fact]
    public void StatusTerminal_NaoPermiteAlteracao()
    {
        var proposta = Proposta.Criar("João", "12345678901", 10000m);
        proposta.AlterarStatus(StatusProposta.Aprovada, null);
        var act = () => proposta.AlterarStatus(StatusProposta.Rejeitada, "motivo");
        act.Should().Throw<ExcecaoDominio>();
    }
}
