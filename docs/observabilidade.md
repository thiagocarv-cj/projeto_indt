# Observabilidade

A plataforma INDT usa **OpenTelemetry** (traces + metrics) e **Serilog** (logs JSON estruturados), exportados de forma vendor-neutral.

## Componentes

| Pilar | Tecnologia |
|-------|------------|
| Traces | OpenTelemetry — ASP.NET Core, HttpClient, EF Core, spans customizados AMQP |
| Metrics | OpenTelemetry — ASP.NET Core, HttpClient |
| Logs | Serilog `RenderedCompactJsonFormatter` no stdout |
| Health | `/health` (liveness), `/health/ready` (PostgreSQL + RabbitMQ) |

Extensão compartilhada: `Shared.Observability.AddIndtObservability()`.

## Configuração

`appsettings.json` / variáveis de ambiente:

```json
{
  "Observability": {
    "OtlpEndpoint": "http://otel-collector:4317",
    "LogLevel": "Information"
  }
}
```

| Variável Docker | Descrição |
|-----------------|-----------|
| `Observability__OtlpEndpoint` | Endpoint OTLP gRPC do collector |
| `Observability__LogLevel` | Nível mínimo Serilog |

Deixe `OtlpEndpoint` vazio para desabilitar export OTLP (útil em testes).

## OpenTelemetry Collector (dev)

Arquivo: [deploy/otel-collector-config.yaml](../deploy/otel-collector-config.yaml)

- Recebe OTLP nas portas **4317** (gRPC) e **4318** (HTTP)
- Dev: exporter `debug` (console)
- Produção: trocar exporter sem alterar código das APIs

## Integração por backend

### Datadog

1. Configure o Datadog Agent com receptor OTLP, ou
2. Adicione exporter `datadog` no OTel Collector apontando para a API key

Variável típica no agente: `DD_OTLP_CONFIG_RECEIVER_PROTOCOLS_HTTP_ENDPOINT=0.0.0.0:4318`

### ELK (Elasticsearch)

1. Adicione exporter `elasticsearch` no collector, ou
2. Colete stdout JSON com Filebeat/Fluent Bit → Elasticsearch Data Streams

Campos úteis nos logs: `@t`, `Level`, `MessageTemplate`, `TraceId`, `SpanId`, `Service`.

### AWS CloudWatch

Use o **ADOT Collector** (AWS Distro for OpenTelemetry) com exporters:

- `awsxray` para traces
- `awsemf` ou `awscloudwatchlogs` para metrics/logs

## Spans customizados

| Componente | Span / tags |
|------------|-------------|
| `RabbitMqPropostaEventPublisher` | `messaging.publish`, `routing_key`, `proposta.id` |
| `PropostaAprovadaBackgroundConsumer` | `messaging.consume`, `queue`, `proposta.id` |
| `PropostaHttpClient` | `http.client`, latência, `proposta.id` |

## Propagação AMQP

O publisher inclui `traceparent` e `tracestate` em `BasicProperties.Headers`. O consumer restaura `ActivityContext` para manter o mesmo `TraceId` ponta a ponta.

Helper: `Shared.Observability.Messaging.AmqpTracePropagation`.

## Exemplo de log estruturado

```json
{
  "@t": "2026-06-07T10:00:00Z",
  "Level": "Information",
  "MessageTemplate": "Contratação ingerida para proposta {PropostaId}",
  "PropostaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "TraceId": "4bf92f3577b34da6a3ce929d0e0e4736",
  "SpanId": "00f067aa0ba902b7",
  "Service": "ContratacaoService"
}
```

## Health checks

```bash
curl http://localhost:5001/health
curl http://localhost:5001/health/ready
curl http://localhost:5002/health/ready
```

`/health/ready` verifica conectividade com PostgreSQL e RabbitMQ antes de considerar o pod pronto.
