using Microsoft.EntityFrameworkCore;
using PropostaService.Application.Portas.Saida;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Infrastructure.Persistence;

public class RepositorioProposta(PropostaDbContext contexto) : IRepositorioProposta
{
    public async Task AdicionarAsync(Proposta proposta, CancellationToken cancellationToken = default)
        => await contexto.Propostas.AddAsync(proposta, cancellationToken);

    public Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => contexto.Propostas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Proposta>> ListarAsync(StatusProposta? status, CancellationToken cancellationToken = default)
    {
        var consulta = contexto.Propostas.AsQueryable();
        if (status.HasValue)
            consulta = consulta.Where(x => x.Status == status.Value);

        return await consulta.OrderByDescending(x => x.DataCriacao).ToListAsync(cancellationToken);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        => contexto.SaveChangesAsync(cancellationToken);
}
