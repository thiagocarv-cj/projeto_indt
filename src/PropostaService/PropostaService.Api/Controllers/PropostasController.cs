using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Domain.Excecoes;

namespace PropostaService.Api.Controllers;

[ApiController]
[Route("api/propostas")]
public class PropostasController(
    ICriarPropostaCasoDeUso criarProposta,
    IObterPropostaCasoDeUso obterProposta,
    IConsultarStatusPropostaCasoDeUso consultarStatus,
    IListarPropostasCasoDeUso listarPropostas,
    IAlterarStatusPropostaCasoDeUso alterarStatus) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RespostaProposta), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] SolicitacaoCriarProposta solicitacao, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await criarProposta.ExecutarAsync(solicitacao, cancellationToken);
            return CreatedAtAction(nameof(Obter), new { id = resultado.Id }, resultado);
        }
        catch (ExcecaoDominio ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? status, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await listarPropostas.ExecutarAsync(status, cancellationToken);
            return Ok(resultado);
        }
        catch (ExcecaoDominio ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespostaProposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await obterProposta.ExecutarAsync(id, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(RespostaStatusProposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarStatus(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await consultarStatus.ExecutarAsync(id, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(RespostaStatusProposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] SolicitacaoAlterarStatusProposta solicitacao,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await alterarStatus.ExecutarAsync(id, solicitacao, cancellationToken);
            return Ok(resultado);
        }
        catch (ExcecaoDominio ex)
        {
            return ex.Message.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)
                ? NotFound(new { error = ex.Message })
                : BadRequest(new { error = ex.Message });
        }
    }
}
