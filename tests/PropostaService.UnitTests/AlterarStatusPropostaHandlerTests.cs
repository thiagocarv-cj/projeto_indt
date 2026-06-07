using FluentAssertions;
using NSubstitute;
using PropostaService.Application.DTOs;
using PropostaService.Application.Handlers;
using PropostaService.Application.Ports;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.UnitTests;

public class TestesAlterarStatusPropostaManipulador
{
    [Fact]
    public async Task ExecutarAsync_Aprovada_DevePublicarEvento()
    {
        var proposta = Proposta.Criar("Maria", "98765432100", 5000m);
        var repositorio = Substitute.For<IRepositorioProposta>();
        repositorio.ObterPorIdAsync(proposta.Id, Arg.Any<CancellationToken>()).Returns(proposta);
        var publicador = Substitute.For<IPublicadorEventosProposta>();
        var manipulador = new AlterarStatusPropostaManipulador(repositorio, publicador);

        await manipulador.ExecutarAsync(proposta.Id, new SolicitacaoAlterarStatusProposta(StatusProposta.Aprovada, null));

        await publicador.Received(1).PublicarStatusAlteradoAsync(proposta, Arg.Any<CancellationToken>());
        await repositorio.Received(1).SalvarAlteracoesAsync(Arg.Any<CancellationToken>());
    }
}
