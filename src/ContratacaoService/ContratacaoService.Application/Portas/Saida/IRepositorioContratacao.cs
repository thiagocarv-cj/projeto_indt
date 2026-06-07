using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Application.Portas.Saida;

public interface IRepositorioContratacao
{
    Task AdicionarAsync(Contratacao contratacao, CancellationToken cancellationToken = default);
    Task<Contratacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<bool> ExistePorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Contratacao> Itens, int TotalRegistros)> ListarPaginadoAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
