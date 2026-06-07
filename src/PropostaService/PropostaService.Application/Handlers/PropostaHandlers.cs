using PropostaService.Application.DTOs;
using PropostaService.Application.Ports;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Excecoes;

namespace PropostaService.Application.Handlers;

public class CriarPropostaManipulador(IRepositorioProposta repositorio) : ICriarPropostaCasoDeUso
{
    public async Task<RespostaProposta> ExecutarAsync(SolicitacaoCriarProposta solicitacao, CancellationToken cancellationToken = default)
    {
        var proposta = Proposta.Criar(solicitacao.NomeSegurado, solicitacao.Cpf, solicitacao.ValorCobertura);
        await repositorio.AdicionarAsync(proposta, cancellationToken);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);
        return Mapear(proposta);
    }

    internal static RespostaProposta Mapear(Proposta proposta) => new(
        proposta.Id,
        proposta.NomeSegurado,
        proposta.Cpf,
        proposta.ValorCobertura,
        proposta.Status,
        proposta.Observacao,
        proposta.DataCriacao,
        proposta.DataAtualizacao);
}

public class ObterPropostaManipulador(IRepositorioProposta repositorio) : IObterPropostaCasoDeUso
{
    public async Task<RespostaProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var proposta = await repositorio.ObterPorIdAsync(id, cancellationToken);
        return proposta is null ? null : CriarPropostaManipulador.Mapear(proposta);
    }
}

public class ConsultarStatusPropostaManipulador(IRepositorioProposta repositorio) : IConsultarStatusPropostaCasoDeUso
{
    public async Task<RespostaStatusProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var proposta = await repositorio.ObterPorIdAsync(id, cancellationToken);
        return proposta is null ? null : MapearStatus(proposta);
    }

    internal static RespostaStatusProposta MapearStatus(Proposta proposta) => new(
        proposta.Id,
        proposta.Status,
        proposta.Observacao,
        proposta.DataCriacao,
        proposta.DataAtualizacao);
}

public class ListarPropostasManipulador(IRepositorioProposta repositorio) : IListarPropostasCasoDeUso
{
    public async Task<IReadOnlyList<RespostaProposta>> ExecutarAsync(string? status, CancellationToken cancellationToken = default)
    {
        StatusProposta? filtro = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusProposta>(status, true, out var statusConvertido))
                throw new ExcecaoDominio($"Status '{status}' inválido.");

            filtro = statusConvertido;
        }

        var propostas = await repositorio.ListarAsync(filtro, cancellationToken);
        return propostas.Select(CriarPropostaManipulador.Mapear).ToList();
    }
}

public class AlterarStatusPropostaManipulador(
    IRepositorioProposta repositorio,
    IPublicadorEventosProposta publicadorEventos) : IAlterarStatusPropostaCasoDeUso
{
    public async Task<RespostaStatusProposta> ExecutarAsync(
        Guid id,
        SolicitacaoAlterarStatusProposta solicitacao,
        CancellationToken cancellationToken = default)
    {
        var proposta = await repositorio.ObterPorIdAsync(id, cancellationToken)
            ?? throw new ExcecaoDominio("Proposta não encontrada.");

        proposta.AlterarStatus(solicitacao.Status, solicitacao.Observacao);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);
        await publicadorEventos.PublicarStatusAlteradoAsync(proposta, cancellationToken);

        return ConsultarStatusPropostaManipulador.MapearStatus(proposta);
    }
}
