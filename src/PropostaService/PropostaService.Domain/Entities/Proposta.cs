using PropostaService.Domain.Enums;
using PropostaService.Domain.Excecoes;

namespace PropostaService.Domain.Entities;

public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeSegurado { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public decimal ValorCobertura { get; private set; }
    public StatusProposta Status { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    private Proposta()
    {
    }

    public static Proposta Criar(string nomeSegurado, string cpf, decimal valorCobertura)
    {
        if (string.IsNullOrWhiteSpace(nomeSegurado))
            throw new ExcecaoDominio("Nome do segurado é obrigatório.");

        if (string.IsNullOrWhiteSpace(cpf))
            throw new ExcecaoDominio("CPF é obrigatório.");

        if (valorCobertura <= 0)
            throw new ExcecaoDominio("Valor de cobertura deve ser maior que zero.");

        var agora = DateTime.UtcNow;
        return new Proposta
        {
            Id = Guid.NewGuid(),
            NomeSegurado = nomeSegurado.Trim(),
            Cpf = cpf.Trim(),
            ValorCobertura = valorCobertura,
            Status = StatusProposta.EmAnalise,
            Observacao = null,
            DataCriacao = agora,
            DataAtualizacao = agora
        };
    }

    public void AlterarStatus(StatusProposta novoStatus, string? observacao)
    {
        ValidarTransicao(novoStatus);

        switch (novoStatus)
        {
            case StatusProposta.Aprovada:
                Observacao = null;
                break;
            case StatusProposta.Rejeitada:
                Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
                break;
            case StatusProposta.Pendencias:
                if (string.IsNullOrWhiteSpace(observacao))
                    throw new ExcecaoDominio("Observação é obrigatória para marcar pendência.");
                Observacao = observacao.Trim();
                break;
        }

        Status = novoStatus;
        DataAtualizacao = DateTime.UtcNow;
    }

    private void ValidarTransicao(StatusProposta novoStatus)
    {
        if (Status is StatusProposta.Aprovada or StatusProposta.Rejeitada)
            throw new ExcecaoDominio($"Proposta com status '{Status}' não permite alteração.");

        var transicoesValidas = Status switch
        {
            StatusProposta.EmAnalise => new[]
            {
                StatusProposta.Aprovada,
                StatusProposta.Rejeitada,
                StatusProposta.Pendencias
            },
            StatusProposta.Pendencias => new[]
            {
                StatusProposta.Aprovada,
                StatusProposta.Rejeitada,
                StatusProposta.Pendencias
            },
            _ => Array.Empty<StatusProposta>()
        };

        if (!transicoesValidas.Contains(novoStatus))
            throw new ExcecaoDominio($"Transição de '{Status}' para '{novoStatus}' não é permitida.");
    }
}
