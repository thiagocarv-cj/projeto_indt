using FluentAssertions;
using NSubstitute;
using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Saida;
using PropostaService.Application.CasosDeUso;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.UnitTests;

public class TestesAlterarStatusPropostaCasoDeUso
{
    [Fact]
    public async Task ExecutarAsync_Aprovada_DevePublicarEvento()
    {
        var proposta = Proposta.Criar("Maria", "98765432100", 5000m);
        var repositorio = Substitute.For<IRepositorioProposta>();
        repositorio.ObterPorIdAsync(proposta.Id, Arg.Any<CancellationToken>()).Returns(proposta);
        var publicador = Substitute.For<IPublicadorEventosProposta>();
        var CasoDeUso = new AlterarStatusPropostaCasoDeUso(repositorio, publicador);

        await CasoDeUso.ExecutarAsync(proposta.Id, new SolicitacaoAlterarStatusProposta(StatusProposta.Aprovada, null));

        await publicador.Received(1).PublicarStatusAlteradoAsync(proposta, Arg.Any<CancellationToken>());
        await repositorio.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }
}
