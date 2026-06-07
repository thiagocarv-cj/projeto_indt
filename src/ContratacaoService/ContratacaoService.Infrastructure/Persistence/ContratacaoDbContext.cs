using Microsoft.EntityFrameworkCore;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Infrastructure.Persistence;

public class ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options) : DbContext(options)
{
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contratacao>(entity =>
        {
            entity.ToTable("contratacoes");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.PropostaId).IsUnique();
            entity.Property(x => x.NomeSegurado).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Cpf).HasMaxLength(14).IsRequired();
            entity.Property(x => x.ValorCobertura).HasPrecision(18, 2);
        });
    }
}
