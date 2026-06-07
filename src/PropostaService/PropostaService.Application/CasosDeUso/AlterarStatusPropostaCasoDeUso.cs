using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Application.Portas.Saida;
using PropostaService.Domain.Excecoes;

namespace PropostaService.Application.CasosDeUso;

public class AlterarStatusPropostaCasoDeUso(
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

        return ConsultarStatusPropostaCasoDeUso.MapearStatus(proposta);
    }
}
