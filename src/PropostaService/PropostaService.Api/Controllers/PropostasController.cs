using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Domain.Excecoes;

namespace PropostaService.Api.Controllers;

/// <summary>Gerencia o ciclo de vida das propostas de seguro.</summary>
[ApiController]
[Route("api/propostas")]
[Produces("application/json")]
public class PropostasController(
    ICriarPropostaCasoDeUso criarProposta,
    IObterPropostaCasoDeUso obterProposta,
    IConsultarStatusPropostaCasoDeUso consultarStatus,
    IListarPropostasCasoDeUso listarPropostas,
    IAlterarStatusPropostaCasoDeUso alterarStatus) : ControllerBase
{
    /// <summary>Cria uma nova proposta de seguro.</summary>
    /// <remarks>
    /// A proposta é criada com status inicial <c>EmAnalise</c>.
    ///
    /// Exemplo de corpo:
    ///
    ///     {
    ///       "nomeSegurado": "Maria Silva",
    ///       "cpf": "12345678901",
    ///       "valorCobertura": 15000.00
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(RespostaProposta), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>Lista propostas, com filtro opcional por status.</summary>
    /// <param name="status">
    /// Filtro opcional. Valores aceitos: <c>EmAnalise</c>, <c>Aprovada</c>, <c>Rejeitada</c>, <c>Pendencias</c>
    /// (case-insensitive). Omita para listar todas.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RespostaProposta>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>Obtém uma proposta pelo identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespostaProposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await obterProposta.ExecutarAsync(id, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    /// <summary>Consulta apenas o status atual de uma proposta.</summary>
    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(RespostaStatusProposta), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarStatus(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await consultarStatus.ExecutarAsync(id, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    /// <summary>Altera o status de uma proposta.</summary>
    /// <remarks>
    /// **Valores de `status` no corpo JSON:**
    ///
    /// | Valor | Quando usar |
    /// |-------|-------------|
    /// | `EmAnalise` | Retornar proposta para análise |
    /// | `Aprovada` | Aprovar — publica evento para contratação |
    /// | `Rejeitada` | Rejeitar definitivamente |
    /// | `Pendencias` | Solicitar documentos/informações (exige `observacao` com mín. 10 caracteres) |
    ///
    /// Exemplo para aprovar:
    ///
    ///     { "status": "Aprovada", "observacao": null }
    ///
    /// Exemplo para pendências:
    ///
    ///     { "status": "Pendencias", "observacao": "Enviar comprovante de residência atualizado." }
    /// </remarks>
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
