using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace Compartilhado.Observabilidade.Mensageria;

public static class PropagacaoRastreamentoRabbitMq
{
    public const string CabecalhoTracePai = "traceparent";
    public const string CabecalhoTraceEstado = "tracestate";

    public static void Injetar(Activity? atividade, IBasicProperties propriedades)
    {
        if (atividade is null)
            return;

        propriedades.Headers ??= new Dictionary<string, object?>();
        propriedades.Headers[CabecalhoTracePai] = atividade.Id;
        if (!string.IsNullOrEmpty(atividade.TraceStateString))
            propriedades.Headers[CabecalhoTraceEstado] = atividade.TraceStateString;
    }

    public static ActivityContext? Extrair(IReadOnlyDictionary<string, object?>? cabecalhos)
    {
        if (cabecalhos is null || !cabecalhos.TryGetValue(CabecalhoTracePai, out var valorBruto))
            return null;

        var tracePai = Encoding.UTF8.GetString(ObterBytesCabecalho(valorBruto));
        cabecalhos.TryGetValue(CabecalhoTraceEstado, out var valorEstadoBruto);
        var traceEstado = valorEstadoBruto is null ? null : Encoding.UTF8.GetString(ObterBytesCabecalho(valorEstadoBruto));

        return ActivityContext.TryParse(tracePai, traceEstado, out var contexto) ? contexto : null;
    }

    private static byte[] ObterBytesCabecalho(object? valor) => valor switch
    {
        byte[] bytes => bytes,
        ReadOnlyMemory<byte> memoria => memoria.ToArray(),
        string texto => Encoding.UTF8.GetBytes(texto),
        _ => Encoding.UTF8.GetBytes(valor?.ToString() ?? string.Empty)
    };
}
