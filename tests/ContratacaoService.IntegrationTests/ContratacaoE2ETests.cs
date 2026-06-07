using System.Net;
using System.Net.Http.Json;
using ContratacaoService.Application.DTOs;
using FluentAssertions;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Enums;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.IntegrationTests;

public class TestesE2EContratacao(AmbienteTesteE2EPlataforma ambiente) : IClassFixture<AmbienteTesteE2EPlataforma>
{
    private void GarantirAmbientePronto()
    {
        if (!ambiente.EstaPronto)
            throw new InvalidOperationException(
                "Docker indisponível — testes de integração requerem Docker Desktop em execução.");
    }

    [Fact]
    public async Task AprovarViaIntermediario_ManipuladorDevePersistirContratacao()
    {
        GarantirAmbientePronto();
        var criacao = await ambiente.ClienteProposta.PostAsJsonAsync("/api/propostas",
            new SolicitacaoCriarProposta("Elena Souza", "12345678901", 25000m));
        criacao.EnsureSuccessStatusCode();
        var proposta = await criacao.Content.ReadFromJsonAsync<RespostaProposta>();
        proposta.Should().NotBeNull();

        var patchIntermediario = await ambiente.ClienteContratacao.PatchAsJsonAsync(
            $"/api/propostas/{proposta!.Id}/status",
            new SolicitacaoAlterarStatusPropostaCompartilhada(StatusPropostaCompartilhado.Aprovada, null));
        patchIntermediario.StatusCode.Should().Be(HttpStatusCode.OK);

        RespostaContratacao? contratacao = null;
        for (var tentativa = 0; tentativa < 20; tentativa++)
        {
            var resposta = await ambiente.ClienteContratacao.GetAsync(
                $"/api/contratacoes/proposta/{proposta.Id}");
            if (resposta.StatusCode == HttpStatusCode.OK)
            {
                contratacao = await resposta.Content.ReadFromJsonAsync<RespostaContratacao>();
                break;
            }

            await Task.Delay(500);
        }

        contratacao.Should().NotBeNull();
        contratacao!.PropostaId.Should().Be(proposta.Id);
        contratacao.NomeSegurado.Should().Be("Elena Souza");

        var lista = await ambiente.ClienteContratacao.GetAsync("/api/contratacoes?page=1&pageSize=10");
        lista.EnsureSuccessStatusCode();
        var paginado = await lista.Content.ReadFromJsonAsync<ResultadoPaginado<RespostaContratacao>>();
        paginado!.TotalRegistros.Should().BeGreaterThan(0);
        paginado.Itens.Should().Contain(x => x.PropostaId == proposta.Id);
    }

    [Fact]
    public async Task ConsultarStatusViaIntermediario_DeveRetornarStatusDoPropostaService()
    {
        GarantirAmbientePronto();
        var criacao = await ambiente.ClienteProposta.PostAsJsonAsync("/api/propostas",
            new SolicitacaoCriarProposta("Felipe Lima", "32165498700", 7000m));
        var proposta = await criacao.Content.ReadFromJsonAsync<RespostaProposta>();

        var respostaStatus = await ambiente.ClienteContratacao.GetAsync($"/api/propostas/{proposta!.Id}/status");
        respostaStatus.EnsureSuccessStatusCode();
        var status = await respostaStatus.Content.ReadFromJsonAsync<RespostaStatusPropostaCompartilhada>();
        status!.Status.Should().Be(StatusPropostaCompartilhado.EmAnalise);
    }
}
