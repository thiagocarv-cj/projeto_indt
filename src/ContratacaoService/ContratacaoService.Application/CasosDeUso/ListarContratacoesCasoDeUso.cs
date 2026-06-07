using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Application.CasosDeUso;

public class ListarContratacoesCasoDeUso(IRepositorioContratacao repositorio) : IListarContratacoesCasoDeUso
{
    public async Task<ResultadoPaginado<RespostaContratacao>> ExecutarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        pagina = pagina <= 0 ? 1 : pagina;
        tamanhoPagina = tamanhoPagina <= 0 ? 20 : Math.Min(tamanhoPagina, 100);

        var (itens, totalRegistros) = await repositorio.ListarPaginadoAsync(pagina, tamanhoPagina, cancellationToken);
        return new ResultadoPaginado<RespostaContratacao>(
            itens.Select(Mapear).ToList(),
            pagina,
            tamanhoPagina,
            totalRegistros);
    }

    internal static RespostaContratacao Mapear(Contratacao contratacao) => new(
        contratacao.Id,
        contratacao.PropostaId,
        contratacao.NomeSegurado,
        contratacao.Cpf,
        contratacao.ValorCobertura,
        contratacao.DataContratacao);
}
