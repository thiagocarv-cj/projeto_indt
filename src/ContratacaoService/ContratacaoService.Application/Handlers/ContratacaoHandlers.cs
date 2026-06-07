using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Ports;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Compartilhado.Contratos.Eventos;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Application.Handlers;

public class IngerirPropostaAprovadaManipulador(
    IRepositorioContratacao repositorio,
    ILogger<IngerirPropostaAprovadaManipulador> logger) : IIngerirPropostaAprovadaCasoDeUso
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

public class ConsultarStatusPropostaIntermediarioManipulador(IClienteServicoProposta cliente) : IConsultarStatusPropostaIntermediarioCasoDeUso
{
    public Task<RespostaStatusPropostaCompartilhada?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default)
        => cliente.ObterStatusAsync(propostaId, cancellationToken);
}

public class AlterarStatusPropostaIntermediarioManipulador(IClienteServicoProposta cliente) : IAlterarStatusPropostaIntermediarioCasoDeUso
{
    public Task<HttpResponseMessage> ExecutarAsync(
        Guid propostaId,
        SolicitacaoAlterarStatusPropostaCompartilhada solicitacao,
        CancellationToken cancellationToken = default)
        => cliente.AlterarStatusAsync(propostaId, solicitacao, cancellationToken);
}

public class ListarContratacoesManipulador(IRepositorioContratacao repositorio) : IListarContratacoesCasoDeUso
{
    public async Task<ResultadoPaginado<RespostaContratacao>> ExecutarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        pagina = pagina <= 0 ? 1 : pagina;
        tamanhoPagina = tamanhoPagina <= 0 ? 20 : Math.Min(tamanhoPagina, 100);

        var (itens, totalRegistros) = await repositorio.ListarPaginadoAsync(pagina, tamanhoPagina, cancellationToken);
        return new ResultadoPaginado<RespostaContratacao>(
            itens.Select(Mapear).ToList(),
            pagina,
            tamanhoPagina,
            totalRegistros);
    }

    internal static RespostaContratacao Mapear(Contratacao contratacao) => new(
        contratacao.Id,
        contratacao.PropostaId,
        contratacao.NomeSegurado,
        contratacao.Cpf,
        contratacao.ValorCobertura,
        contratacao.DataContratacao);
}

public class ObterContratacaoManipulador(IRepositorioContratacao repositorio) : IObterContratacaoCasoDeUso
{
    public async Task<RespostaContratacao?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contratacao = await repositorio.ObterPorIdAsync(id, cancellationToken);
        return contratacao is null ? null : ListarContratacoesManipulador.Mapear(contratacao);
    }
}

public class ObterContratacaoPorPropostaManipulador(IRepositorioContratacao repositorio) : IObterContratacaoPorPropostaCasoDeUso
{
    public async Task<RespostaContratacao?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        var contratacao = await repositorio.ObterPorPropostaIdAsync(propostaId, cancellationToken);
        return contratacao is null ? null : ListarContratacoesManipulador.Mapear(contratacao);
    }
}
