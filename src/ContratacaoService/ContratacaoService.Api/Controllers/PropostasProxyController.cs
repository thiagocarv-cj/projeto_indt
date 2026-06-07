using ContratacaoService.Application.Ports;
using ContratacaoService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Propostas;

namespace ContratacaoService.Api.Controllers;

[ApiController]
[Route("api/propostas")]
public class PropostasIntermediarioController(
    IConsultarStatusPropostaIntermediarioCasoDeUso consultarStatus,
    IAlterarStatusPropostaIntermediarioCasoDeUso alterarStatus) : ControllerBase
{
    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> ConsultarStatus(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await consultarStatus.ExecutarAsync(id, cancellationToken);
            return resultado is null ? NotFound() : Ok(resultado);
        }
        catch (ExcecaoServicoPropostaIndisponivel)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "PropostaService indisponível." });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] SolicitacaoAlterarStatusPropostaCompartilhada solicitacao,
        CancellationToken cancellationToken)
    {
        try
        {
            var respostaHttp = await alterarStatus.ExecutarAsync(id, solicitacao, cancellationToken);
            var conteudo = await respostaHttp.Content.ReadAsStringAsync(cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)respostaHttp.StatusCode,
                Content = conteudo,
                ContentType = "application/json"
            };
        }
        catch (ExcecaoServicoPropostaIndisponivel)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "PropostaService indisponível." });
        }
    }
}
