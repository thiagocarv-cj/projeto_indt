using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Persistence;

public class PropostaDbContext(DbContextOptions<PropostaDbContext> options) : DbContext(options)
{
    public DbSet<Proposta> Propostas => Set<Proposta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proposta>(entity =>
        {
            entity.ToTable("propostas");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.NomeSegurado).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Cpf).HasMaxLength(14).IsRequired();
            entity.Property(x => x.ValorCobertura).HasPrecision(18, 2);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.Observacao).HasMaxLength(500);
        });
    }
}
