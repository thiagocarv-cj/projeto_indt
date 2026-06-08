using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PropostaService.Api.Documentacao;

public static class ConfiguracaoSwagger
{
    public static IServiceCollection AdicionarDocumentacaoSwaggerProposta(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PropostaService API",
                Version = "v1",
                Description = """
                    API responsável pelo ciclo de vida das **propostas de seguro**.

                    ## Status de proposta (`status`)

                    | Valor | Significado |
                    |-------|-------------|
                    | `EmAnalise` | Status inicial após criação — aguardando análise |
                    | `Aprovada` | Proposta aprovada; publica evento para contratação |
                    | `Rejeitada` | Proposta rejeitada definitivamente |
                    | `Pendencias` | Exige `observacao` com **mínimo 10 caracteres** |

                    Os valores são **case-insensitive** no filtro de listagem (`GET /api/propostas?status=...`).
                    No corpo JSON use exatamente os nomes acima (ex.: `"Aprovada"`).

                    ## Fluxo típico

                    1. `POST /api/propostas` — criar proposta (`EmAnalise`)
                    2. `PATCH /api/propostas/{id}/status` — aprovar, rejeitar ou marcar pendências
                    3. `GET /api/propostas/{id}/status` — consultar status atual
                    """
            });

            options.SchemaFilter<FiltroDocumentacaoEnum>();
            options.OperationFilter<FiltroExemplosProposta>();

            IncluirComentariosXml(options,
                typeof(Program).Assembly,
                typeof(Application.DTOs.SolicitacaoCriarProposta).Assembly,
                typeof(Domain.Enums.StatusProposta).Assembly);
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

internal sealed class FiltroExemplosProposta : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metodo = context.ApiDescription.HttpMethod?.ToUpperInvariant();
        var caminho = context.ApiDescription.RelativePath ?? string.Empty;

        if (metodo == "POST" && caminho == "api/propostas")
        {
            DefinirExemploCorpo(operation, """
                {
                  "nomeSegurado": "Maria Silva",
                  "cpf": "12345678901",
                  "valorCobertura": 15000.00
                }
                """);
            return;
        }

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

        if (metodo == "GET" && caminho == "api/propostas")
        {
            var parametroStatus = operation.Parameters.FirstOrDefault(p => p.Name == "status");
            if (parametroStatus is not null)
            {
                parametroStatus.Description = "Filtra propostas por status. Valores: EmAnalise, Aprovada, Rejeitada, Pendencias.";
                parametroStatus.Required = false;
                parametroStatus.Schema ??= new OpenApiSchema { Type = "string" };
                parametroStatus.Schema.Example = new OpenApiString("EmAnalise");
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
