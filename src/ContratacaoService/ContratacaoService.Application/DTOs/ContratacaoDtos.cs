namespace ContratacaoService.Application.DTOs;

/// <summary>Contratação gerada a partir de uma proposta aprovada.</summary>
public record RespostaContratacao(
    Guid Id,
    Guid PropostaId,
    string NomeSegurado,
    string Cpf,
    decimal ValorCobertura,
    DateTime DataContratacao);

/// <summary>Resultado paginado de uma consulta.</summary>
/// <param name="Itens">Registros da página atual.</param>
/// <param name="Pagina">Número da página (inicia em 1).</param>
/// <param name="TamanhoPagina">Quantidade de itens por página.</param>
/// <param name="TotalRegistros">Total de registros em todas as páginas.</param>
public record ResultadoPaginado<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalRegistros);
