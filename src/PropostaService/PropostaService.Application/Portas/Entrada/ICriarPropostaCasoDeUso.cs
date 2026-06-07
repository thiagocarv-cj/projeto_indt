using PropostaService.Application.DTOs;

namespace PropostaService.Application.Portas.Entrada;

public interface ICriarPropostaCasoDeUso
{
    Task<RespostaProposta> ExecutarAsync(SolicitacaoCriarProposta solicitacao, CancellationToken cancellationToken = default);
}
