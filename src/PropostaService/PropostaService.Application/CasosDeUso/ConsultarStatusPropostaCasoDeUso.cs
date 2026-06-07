using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Application.Portas.Saida;
using PropostaService.Domain.Entities;

namespace PropostaService.Application.CasosDeUso;

public class ConsultarStatusPropostaCasoDeUso(IRepositorioProposta repositorio) : IConsultarStatusPropostaCasoDeUso
{
    public async Task<RespostaStatusProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var proposta = await repositorio.ObterPorIdAsync(id, cancellationToken);
        return proposta is null ? null : MapearStatus(proposta);
    }

    internal static RespostaStatusProposta MapearStatus(Proposta proposta) => new(
        proposta.Id,
        proposta.Status,
        proposta.Observacao,
        proposta.DataCriacao,
        proposta.DataAtualizacao);
}
