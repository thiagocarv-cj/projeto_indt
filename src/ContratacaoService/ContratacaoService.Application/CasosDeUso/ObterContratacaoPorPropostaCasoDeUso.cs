using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;

namespace ContratacaoService.Application.CasosDeUso;

public class ObterContratacaoPorPropostaCasoDeUso(IRepositorioContratacao repositorio) : IObterContratacaoPorPropostaCasoDeUso
{
    public async Task<RespostaContratacao?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        var contratacao = await repositorio.ObterPorPropostaIdAsync(propostaId, cancellationToken);
        return contratacao is null ? null : ListarContratacoesCasoDeUso.Mapear(contratacao);
    }
}
