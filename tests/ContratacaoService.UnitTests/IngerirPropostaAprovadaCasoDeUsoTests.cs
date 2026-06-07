using FluentAssertions;
using NSubstitute;
using ContratacaoService.Application.Portas.Saida;
using ContratacaoService.Application.CasosDeUso;
using Compartilhado.Contratos.Eventos;

namespace ContratacaoService.UnitTests;

public class IngerirPropostaAprovadaCasoDeUsoTests
{
    [Fact]
    public async Task ExecutarAsync_PropostaDuplicada_NaoAdicionaNovamente()
    {
        var repositorio = Substitute.For<IRepositorioContratacao>();
        repositorio.ExistePorPropostaIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        var CasoDeUso = new IngerirPropostaAprovadaCasoDeUso(repositorio, Substitute.For<Microsoft.Extensions.Logging.ILogger<IngerirPropostaAprovadaCasoDeUso>>());
        var evento = new EventoPropostaAprovada(Guid.NewGuid(), "João", "123", 1000m, DateTime.UtcNow);

        await CasoDeUso.ExecutarAsync(evento);

        await repositorio.DidNotReceive().AdicionarAsync(Arg.Any<ContratacaoService.Domain.Entities.Contratacao>(), Arg.Any<CancellationToken>());
    }
}
