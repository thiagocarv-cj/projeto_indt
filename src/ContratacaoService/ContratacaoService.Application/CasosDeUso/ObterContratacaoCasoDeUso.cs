using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;

namespace ContratacaoService.Application.CasosDeUso;

public class ObterContratacaoCasoDeUso(IRepositorioContratacao repositorio) : IObterContratacaoCasoDeUso
{
    public async Task<RespostaContratacao?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contratacao = await repositorio.ObterPorIdAsync(id, cancellationToken);
        return contratacao is null ? null : ListarContratacoesCasoDeUso.Mapear(contratacao);
    }
}
