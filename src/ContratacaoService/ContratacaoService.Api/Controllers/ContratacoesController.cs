using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Portas.Entrada;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.Api.Controllers;

/// <summary>Consulta contratações geradas a partir de propostas aprovadas.</summary>
[ApiController]
[Route("api/contratacoes")]
[Produces("application/json")]
public class ContratacoesController(
    IListarContratacoesCasoDeUso listarContratacoes,
    IObterContratacaoCasoDeUso obterContratacao,
    IObterContratacaoPorPropostaCasoDeUso obterPorProposta) : ControllerBase
{
    /// <summary>Lista contratações com paginação.</summary>
    /// <param name="pagina">Número da página (padrão: 1).</param>
    /// <param name="tamanhoPagina">Itens por página (padrão: 20, máximo: 100).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<RespostaContratacao>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery(Name = "page")] int pagina = 1,
        [FromQuery(Name = "pageSize")] int tamanhoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listarContratacoes.ExecutarAsync(pagina, tamanhoPagina, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Obtém uma contratação pelo identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespostaContratacao), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await obterContratacao.ExecutarAsync(id, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    /// <summary>Obtém a contratação vinculada a uma proposta aprovada.</summary>
    /// <param name="propostaId">Identificador da proposta no PropostaService.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpGet("proposta/{propostaId:guid}")]
    [ProducesResponseType(typeof(RespostaContratacao), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorProposta(Guid propostaId, CancellationToken cancellationToken)
    {
        var resultado = await obterPorProposta.ExecutarAsync(propostaId, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }
}
