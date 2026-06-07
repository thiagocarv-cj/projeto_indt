using Compartilhado.Contratos.Eventos;

namespace ContratacaoService.Application.Portas.Entrada;

public interface IIngerirPropostaAprovadaCasoDeUso
{
    Task ExecutarAsync(EventoPropostaAprovada evento, CancellationToken cancellationToken = default);
}
