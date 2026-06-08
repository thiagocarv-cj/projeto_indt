using ContratacaoService.Application.Portas.Saida;
using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Persistence;

public class RepositorioContratacao(ContratacaoDbContext contexto) : IRepositorioContratacao
{
    public async Task AdicionarAsync(Contratacao contratacao, CancellationToken cancellationToken = default)
        => await contexto.Contratacoes.AddAsync(contratacao, cancellationToken);

    public Task<Contratacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => contexto.Contratacoes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default)
        => contexto.Contratacoes.FirstOrDefaultAsync(x => x.PropostaId == propostaId, cancellationToken);

    public Task<bool> ExistePorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default)
        => contexto.Contratacoes.AnyAsync(x => x.PropostaId == propostaId, cancellationToken);

    public async Task<(IReadOnlyList<Contratacao> Itens, int TotalRegistros)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Contratacoes.OrderByDescending(x => x.DataContratacao);
        var totalRegistros = await consulta.CountAsync(cancellationToken);
        var itens = await consulta.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync(cancellationToken);
        return (itens, totalRegistros);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        => contexto.SaveChangesAsync(cancellationToken);
}
