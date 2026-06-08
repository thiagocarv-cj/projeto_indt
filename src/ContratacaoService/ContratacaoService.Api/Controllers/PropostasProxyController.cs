using ContratacaoService.Application.Excecoes;
using ContratacaoService.Application.Portas.Entrada;
using Microsoft.AspNetCore.Mvc;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Api.Controllers;

/// <summary>Intermediário HTTP para operações de status no PropostaService.</summary>
[ApiController]
[Route("api/propostas")]
[Produces("application/json")]
public class PropostasIntermediarioController(
    IConsultarStatusPropostaIntermediarioCasoDeUso consultarStatus,
    IAlterarStatusPropostaIntermediarioCasoDeUso alterarStatus) : ControllerBase
{
    /// <summary>Consulta o status de uma proposta no PropostaService.</summary>
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(RespostaStatusPropostaCompartilhada), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
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

    /// <summary>Altera o status de uma proposta via PropostaService.</summary>
    /// <remarks>
    /// Repassa a solicitação ao PropostaService. Ao aprovar (`"status": "Aprovada"`), o evento é publicado
    /// no RabbitMQ e a contratação é criada de forma assíncrona.
    ///
    /// **Valores de `status`:** `EmAnalise`, `Aprovada`, `Rejeitada`, `Pendencias`.
    ///
    /// Para `Pendencias`, informe `observacao` com no mínimo 10 caracteres.
    /// </remarks>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
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
