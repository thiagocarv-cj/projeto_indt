using ContratacaoService.Domain.Entities;
using Shared.Contracts.Propostas;

namespace ContratacaoService.Application.Ports;

public interface IRepositorioContratacao
{
    Task AdicionarAsync(Contratacao contratacao, CancellationToken cancellationToken = default);
    Task<Contratacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<bool> ExistePorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Contratacao> Itens, int TotalRegistros)> ListarPaginadoAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}

public interface IClienteServicoProposta
{
    Task<RespostaStatusPropostaCompartilhada?> ObterStatusAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> AlterarStatusAsync(Guid propostaId, SolicitacaoAlterarStatusPropostaCompartilhada solicitacao, CancellationToken cancellationToken = default);
}

public class ExcecaoServicoPropostaIndisponivel : Exception
{
    public ExcecaoServicoPropostaIndisponivel(string message, Exception? inner = null) : base(message, inner)
    {
    }
}
