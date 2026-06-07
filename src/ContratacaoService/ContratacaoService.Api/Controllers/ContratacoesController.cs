using ContratacaoService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.Api.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController(
    IListarContratacoesCasoDeUso listarContratacoes,
    IObterContratacaoCasoDeUso obterContratacao,
    IObterContratacaoPorPropostaCasoDeUso obterPorProposta) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery(Name = "page")] int pagina = 1,
        [FromQuery(Name = "pageSize")] int tamanhoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await listarContratacoes.ExecutarAsync(pagina, tamanhoPagina, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await obterContratacao.ExecutarAsync(id, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }

    [HttpGet("proposta/{propostaId:guid}")]
    public async Task<IActionResult> ObterPorProposta(Guid propostaId, CancellationToken cancellationToken)
    {
        var resultado = await obterPorProposta.ExecutarAsync(propostaId, cancellationToken);
        return resultado is null ? NotFound() : Ok(resultado);
    }
}
