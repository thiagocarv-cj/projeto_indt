using PropostaService.Application.DTOs;

namespace PropostaService.Application.UseCases;

public interface ICriarPropostaCasoDeUso
{
    Task<RespostaProposta> ExecutarAsync(SolicitacaoCriarProposta solicitacao, CancellationToken cancellationToken = default);
}

public interface IObterPropostaCasoDeUso
{
    Task<RespostaProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IConsultarStatusPropostaCasoDeUso
{
    Task<RespostaStatusProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarPropostasCasoDeUso
{
    Task<IReadOnlyList<RespostaProposta>> ExecutarAsync(string? status, CancellationToken cancellationToken = default);
}

public interface IAlterarStatusPropostaCasoDeUso
{
    Task<RespostaStatusProposta> ExecutarAsync(Guid id, SolicitacaoAlterarStatusProposta solicitacao, CancellationToken cancellationToken = default);
}
