using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

namespace Compartilhado.Observabilidade;

public static class ExtensoesObservabilidade
{
    public static WebApplicationBuilder AdicionarObservabilidadeIndt(this WebApplicationBuilder builder, string nomeServico)
    {
        var endpointOtlp = builder.Configuration["Observabilidade:EndpointOtlp"];
        var nivelLog = builder.Configuration["Observabilidade:NivelLog"] ?? "Information";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(Enum.TryParse<Serilog.Events.LogEventLevel>(nivelLog, true, out var nivel)
                ? nivel
                : Serilog.Events.LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Servico", nomeServico)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(nomeServico))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(endpointOtlp))
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(endpointOtlp));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (!string.IsNullOrWhiteSpace(endpointOtlp))
                    metrics.AddOtlpExporter(o => o.Endpoint = new Uri(endpointOtlp));
            });

        return builder;
    }
}
