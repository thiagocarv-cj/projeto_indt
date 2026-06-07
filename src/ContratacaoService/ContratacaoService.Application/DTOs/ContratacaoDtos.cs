namespace ContratacaoService.Application.DTOs;

public record RespostaContratacao(
    Guid Id,
    Guid PropostaId,
    string NomeSegurado,
    string Cpf,
    decimal ValorCobertura,
    DateTime DataContratacao);

public record ResultadoPaginado<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalRegistros);
