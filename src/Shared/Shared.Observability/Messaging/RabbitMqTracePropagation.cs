using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace Shared.Observability.Messaging;

public static class PropagacaoRastreamentoRabbitMq
{
    public const string TraceParentHeader = "traceparent";
    public const string TraceStateHeader = "tracestate";

    public static void Injetar(Activity? activity, IBasicProperties properties)
    {
        if (activity is null)
            return;

        properties.Headers ??= new Dictionary<string, object?>();
        properties.Headers[TraceParentHeader] = activity.Id;
        if (!string.IsNullOrEmpty(activity.TraceStateString))
            properties.Headers[TraceStateHeader] = activity.TraceStateString;
    }

    public static ActivityContext? Extrair(IReadOnlyDictionary<string, object?>? headers)
    {
        if (headers is null || !headers.TryGetValue(TraceParentHeader, out var raw))
            return null;

        var traceParent = Encoding.UTF8.GetString(ObterBytesCabecalho(raw));
        headers.TryGetValue(TraceStateHeader, out var traceStateRaw);
        var traceState = traceStateRaw is null ? null : Encoding.UTF8.GetString(ObterBytesCabecalho(traceStateRaw));

        return ActivityContext.TryParse(traceParent, traceState, out var context) ? context : null;
    }

    private static byte[] ObterBytesCabecalho(object? value) => value switch
    {
        byte[] bytes => bytes,
        ReadOnlyMemory<byte> memory => memory.ToArray(),
        string s => Encoding.UTF8.GetBytes(s),
        _ => Encoding.UTF8.GetBytes(value?.ToString() ?? string.Empty)
    };
}
