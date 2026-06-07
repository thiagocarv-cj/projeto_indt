using PropostaService.Application.DTOs;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Application.Portas.Saida;
using PropostaService.Domain.Entities;

namespace PropostaService.Application.CasosDeUso;

public class CriarPropostaCasoDeUso(IRepositorioProposta repositorio) : ICriarPropostaCasoDeUso
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
