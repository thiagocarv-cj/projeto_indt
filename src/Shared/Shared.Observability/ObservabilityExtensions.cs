using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Compact;

namespace Shared.Observability;

public static class ExtensoesObservabilidade
{
    public static WebApplicationBuilder AdicionarObservabilidadeIndt(this WebApplicationBuilder builder, string serviceName)
    {
        var otlpEndpoint = builder.Configuration["Observability:OtlpEndpoint"];
        var logLevel = builder.Configuration["Observability:LogLevel"] ?? "Information";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(Enum.TryParse<Serilog.Events.LogEventLevel>(logLevel, true, out var level)
                ? level
                : Serilog.Events.LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            });

        return builder;
    }
}
