using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;
using ContratacaoService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Compartilhado.Contratos.Eventos;

namespace ContratacaoService.Application.CasosDeUso;

public class IngerirPropostaAprovadaCasoDeUso(
    IRepositorioContratacao repositorio,
    ILogger<IngerirPropostaAprovadaCasoDeUso> logger) : IIngerirPropostaAprovadaCasoDeUso
{
    public async Task ExecutarAsync(EventoPropostaAprovada evento, CancellationToken cancellationToken = default)
    {
        if (await repositorio.ExistePorPropostaIdAsync(evento.PropostaId, cancellationToken))
        {
            logger.LogInformation("Contratação já existente para proposta {PropostaId}", evento.PropostaId);
            return;
        }

        var contratacao = Contratacao.Criar(
            evento.PropostaId,
            evento.NomeSegurado,
            evento.Cpf,
            evento.ValorCobertura,
            DateTime.UtcNow);

        await repositorio.AdicionarAsync(contratacao, cancellationToken);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);

        logger.LogInformation("Contratação ingerida para proposta {PropostaId}", evento.PropostaId);
    }
}
