using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using ContratacaoService.Application.Portas.Saida;
using ContratacaoService.Application.CasosDeUso;
using FluentAssertions;
using NSubstitute;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.UnitTests;

public class TestesAlterarStatusPropostaIntermediarioCasoDeUso
{
    [Fact]
    public async Task ExecutarAsync_DeveRepassarRespostaDoPropostaService()
    {
        var cliente = Substitute.For<IClienteServicoProposta>();
        var respostaEsperada = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"status\":\"Aprovada\"}", Encoding.UTF8, "application/json")
        };
        var solicitacao = new SolicitacaoAlterarStatusPropostaCompartilhada(StatusPropostaCompartilhado.Aprovada, null);
        var propostaId = Guid.NewGuid();

        cliente.AlterarStatusAsync(propostaId, solicitacao, Arg.Any<CancellationToken>()).Returns(respostaEsperada);

        var CasoDeUso = new AlterarStatusPropostaIntermediarioCasoDeUso(cliente);
        var resposta = await CasoDeUso.ExecutarAsync(propostaId, solicitacao);

        resposta.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await resposta.Content.ReadAsStringAsync();
        JsonDocument.Parse(json).RootElement.GetProperty("status").GetString().Should().Be("Aprovada");
        await cliente.Received(1).AlterarStatusAsync(propostaId, solicitacao, Arg.Any<CancellationToken>());
    }
}
