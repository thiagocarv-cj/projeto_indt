using ContratacaoService.Application.DTOs;

namespace ContratacaoService.Application.Portas.Entrada;

public interface IListarContratacoesCasoDeUso
{
    Task<ResultadoPaginado<RespostaContratacao>> ExecutarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
}
