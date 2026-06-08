using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContratacaoService.Api.Documentacao;

public static class ConfiguracaoSwagger
{
    public static IServiceCollection AdicionarDocumentacaoSwaggerContratacao(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ContratacaoService API",
                Version = "v1",
                Description = """
                    API de **contratações** e **intermediário HTTP** para o PropostaService.

                    ## Status de proposta (`status`)

                    Usado no proxy `PATCH /api/propostas/{id}/status` (repassado ao PropostaService):

                    | Valor | Significado |
                    |-------|-------------|
                    | `EmAnalise` | Aguardando análise |
                    | `Aprovada` | Aprovada — gera contratação via fila RabbitMQ |
                    | `Rejeitada` | Rejeitada definitivamente |
                    | `Pendencias` | Exige `observacao` com **mínimo 10 caracteres** |

                    ## Fluxo típico

                    1. Aprovar proposta via `PATCH /api/propostas/{id}/status` com `"status": "Aprovada"`
                    2. Aguardar processamento assíncrono (fila `propostas.aprovadas`)
                    3. Consultar contratação em `GET /api/contratacoes/proposta/{propostaId}`
                    """
            });

            options.SchemaFilter<FiltroDocumentacaoEnum>();
            options.OperationFilter<FiltroExemplosContratacao>();

            IncluirComentariosXml(options,
                typeof(Program).Assembly,
                typeof(Application.DTOs.RespostaContratacao).Assembly,
                typeof(Compartilhado.Contratos.Propostas.StatusPropostaCompartilhado).Assembly);
        });

        return services;
    }

    private static void IncluirComentariosXml(SwaggerGenOptions options, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var caminhoXml = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
            if (File.Exists(caminhoXml))
                options.IncludeXmlComments(caminhoXml, includeControllerXmlComments: true);
        }
    }
}

internal sealed class FiltroDocumentacaoEnum : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
            return;

        var linhas = new List<string> { "Valores aceitos:" };
        foreach (var nome in Enum.GetNames(context.Type))
        {
            var campo = context.Type.GetField(nome);
            var descricao = campo?.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description;
            linhas.Add(descricao is null ? $"- `{nome}`" : $"- `{nome}` — {descricao}");
        }

        schema.Description = string.Join(Environment.NewLine, linhas);
        schema.Example = new OpenApiString(Enum.GetNames(context.Type).First());
    }
}

internal sealed class FiltroExemplosContratacao : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metodo = context.ApiDescription.HttpMethod?.ToUpperInvariant();
        var caminho = context.ApiDescription.RelativePath ?? string.Empty;

        if (metodo == "PATCH" && caminho.Contains("/status", StringComparison.Ordinal))
        {
            DefinirExemploCorpo(operation, """
                {
                  "status": "Aprovada",
                  "observacao": null
                }
                """);
            AdicionarNota(operation,
                "Exemplos de `status`: `EmAnalise`, `Aprovada`, `Rejeitada`, `Pendencias`. " +
                "Para `Pendencias`, informe `observacao` com no mínimo 10 caracteres.");
            return;
        }

        if (metodo == "GET" && caminho == "api/contratacoes")
        {
            foreach (var parametro in operation.Parameters)
            {
                if (parametro.Name == "page")
                {
                    parametro.Description = "Número da página (inicia em 1).";
                    parametro.Schema.Example = new OpenApiInteger(1);
                }
                else if (parametro.Name == "pageSize")
                {
                    parametro.Description = "Quantidade de itens por página (máximo 100).";
                    parametro.Schema.Example = new OpenApiInteger(20);
                }
            }
        }
    }

    private static void DefinirExemploCorpo(OpenApiOperation operation, string json)
    {
        if (operation.RequestBody?.Content.TryGetValue("application/json", out var media) == true)
            media.Example = OpenApiAnyFactory.CreateFromJson(json);
    }

    private static void AdicionarNota(OpenApiOperation operation, string texto)
    {
        operation.Description = string.IsNullOrWhiteSpace(operation.Description)
            ? texto
            : $"{operation.Description}{Environment.NewLine}{Environment.NewLine}{texto}";
    }
}
