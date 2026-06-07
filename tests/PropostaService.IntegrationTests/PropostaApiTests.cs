using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PropostaService.Application.DTOs;
using PropostaService.Domain.Enums;
using Xunit;

namespace PropostaService.IntegrationTests;

public class TestesApiProposta(AmbienteTesteApiProposta ambiente) : IClassFixture<AmbienteTesteApiProposta>
{
    [SkippableFact]
    public async Task CriarProposta_DeveRetornar201()
    {
        Skip.IfNot(ambiente.EstaPronto, ambiente.MotivoIndisponibilidade);

        var resposta = await ambiente.Cliente.PostAsJsonAsync("/api/propostas",
            new SolicitacaoCriarProposta("Ana Silva", "11122233344", 15000m));

        resposta.StatusCode.Should().Be(HttpStatusCode.Created);
        var corpo = await resposta.Content.ReadFromJsonAsync<RespostaProposta>();
        corpo!.Status.Should().Be(StatusProposta.EmAnalise);
    }

    [SkippableFact]
    public async Task FluxoCompleto_CriarConsultarAprovar_DeveRetornarAprovada()
    {
        Skip.IfNot(ambiente.EstaPronto, ambiente.MotivoIndisponibilidade);

        var criacao = await ambiente.Cliente.PostAsJsonAsync("/api/propostas",
            new SolicitacaoCriarProposta("Bruno Costa", "99988877766", 12000m));
        var proposta = await criacao.Content.ReadFromJsonAsync<RespostaProposta>();
        proposta.Should().NotBeNull();

        var consultaStatus = await ambiente.Cliente.GetAsync($"/api/propostas/{proposta!.Id}/status");
        consultaStatus.StatusCode.Should().Be(HttpStatusCode.OK);

        var aprovacao = await ambiente.Cliente.PatchAsJsonAsync($"/api/propostas/{proposta.Id}/status",
            new SolicitacaoAlterarStatusProposta(StatusProposta.Aprovada, null));
        aprovacao.StatusCode.Should().Be(HttpStatusCode.OK);

        var status = await aprovacao.Content.ReadFromJsonAsync<RespostaStatusProposta>();
        status!.Status.Should().Be(StatusProposta.Aprovada);
    }

    [SkippableFact]
    public async Task PendenciaSemObservacaoValida_DeveRetornar400()
    {
        Skip.IfNot(ambiente.EstaPronto, ambiente.MotivoIndisponibilidade);

        var criacao = await ambiente.Cliente.PostAsJsonAsync("/api/propostas",
            new SolicitacaoCriarProposta("Carlos", "55566677788", 8000m));
        var proposta = await criacao.Content.ReadFromJsonAsync<RespostaProposta>();
        proposta.Should().NotBeNull();

        var patch = await ambiente.Cliente.PatchAsJsonAsync($"/api/propostas/{proposta!.Id}/status",
            new SolicitacaoAlterarStatusProposta(StatusProposta.Pendencias, "curta"));

        patch.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [SkippableFact]
    public async Task PendenciaComObservacaoValida_DeveRetornar200()
    {
        Skip.IfNot(ambiente.EstaPronto, ambiente.MotivoIndisponibilidade);

        var criacao = await ambiente.Cliente.PostAsJsonAsync("/api/propostas",
            new SolicitacaoCriarProposta("Diana", "44455566677", 9000m));
        var proposta = await criacao.Content.ReadFromJsonAsync<RespostaProposta>();

        var patch = await ambiente.Cliente.PatchAsJsonAsync($"/api/propostas/{proposta!.Id}/status",
            new SolicitacaoAlterarStatusProposta(StatusProposta.Pendencias,
                "Documentação incompleta — favor enviar comprovante de renda atualizado."));

        patch.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
