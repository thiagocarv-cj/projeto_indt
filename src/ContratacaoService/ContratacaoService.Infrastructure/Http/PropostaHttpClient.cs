using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ContratacaoService.Application.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Infrastructure.Http;

public class ClienteHttpProposta(
    HttpClient clienteHttp,
    IOptions<OpcoesServicoProposta> opcoes,
    ILogger<ClienteHttpProposta> logger) : IClienteServicoProposta
{
    private static readonly JsonSerializerOptions OpcoesJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public async Task<RespostaStatusPropostaCompartilhada?> ObterStatusAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var resposta = await clienteHttp.GetAsync(MontarUrlStatusProposta(propostaId), cancellationToken);

            if (resposta.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!resposta.IsSuccessStatusCode)
                throw new ExcecaoServicoPropostaIndisponivel($"PropostaService retornou {(int)resposta.StatusCode}.");

            return await resposta.Content.ReadFromJsonAsync<RespostaStatusPropostaCompartilhada>(OpcoesJson, cancellationToken);
        }
        catch (ExcecaoServicoPropostaIndisponivel)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao consultar status da proposta {PropostaId}", propostaId);
            throw new ExcecaoServicoPropostaIndisponivel("PropostaService indisponível.", ex);
        }
    }

    private string MontarUrlStatusProposta(Guid propostaId)
        => $"{opcoes.Value.BaseUrl.TrimEnd('/')}/api/propostas/{propostaId}/status";

    public async Task<HttpResponseMessage> AlterarStatusAsync(
        Guid propostaId,
        SolicitacaoAlterarStatusPropostaCompartilhada solicitacao,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await clienteHttp.PatchAsJsonAsync(
                MontarUrlStatusProposta(propostaId),
                solicitacao,
                OpcoesJson,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao alterar status da proposta {PropostaId}", propostaId);
            throw new ExcecaoServicoPropostaIndisponivel("PropostaService indisponível.", ex);
        }
    }
}
