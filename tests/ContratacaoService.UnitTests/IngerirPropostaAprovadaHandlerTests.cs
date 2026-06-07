using FluentAssertions;
using NSubstitute;
using ContratacaoService.Application.Handlers;
using ContratacaoService.Application.Ports;
using Shared.Contracts.Eventos;

namespace ContratacaoService.UnitTests;

public class IngerirPropostaAprovadaManipuladorTests
{
    [Fact]
    public async Task ExecutarAsync_PropostaDuplicada_NaoAdicionaNovamente()
    {
        var repositorio = Substitute.For<IRepositorioContratacao>();
        repositorio.ExistePorPropostaIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        var manipulador = new IngerirPropostaAprovadaManipulador(repositorio, Substitute.For<Microsoft.Extensions.Logging.ILogger<IngerirPropostaAprovadaManipulador>>());
        var evento = new EventoPropostaAprovada(Guid.NewGuid(), "João", "123", 1000m, DateTime.UtcNow);

        await manipulador.ExecutarAsync(evento);

        await repositorio.DidNotReceive().AdicionarAsync(Arg.Any<ContratacaoService.Domain.Entities.Contratacao>(), Arg.Any<CancellationToken>());
    }
}
