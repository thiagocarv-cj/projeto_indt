using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Application.Portas.Saida;

namespace PropostaService.Application.CasosDeUso;

public class ObterPropostaCasoDeUso(IRepositorioProposta repositorio) : IObterPropostaCasoDeUso
{
    public async Task<RespostaProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var proposta = await repositorio.ObterPorIdAsync(id, cancellationToken);
        return proposta is null ? null : CriarPropostaCasoDeUso.Mapear(proposta);
    }
}
