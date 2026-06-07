using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Application.Portas.Saida;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Excecoes;

namespace PropostaService.Application.CasosDeUso;

public class ListarPropostasCasoDeUso(IRepositorioProposta repositorio) : IListarPropostasCasoDeUso
{
    public async Task<IReadOnlyList<RespostaProposta>> ExecutarAsync(string? status, CancellationToken cancellationToken = default)
    {
        StatusProposta? filtro = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusProposta>(status, true, out var statusConvertido))
                throw new ExcecaoDominio($"Status '{status}' inválido.");

            filtro = statusConvertido;
        }

        var propostas = await repositorio.ListarAsync(filtro, cancellationToken);
        return propostas.Select(CriarPropostaCasoDeUso.Mapear).ToList();
    }
}
