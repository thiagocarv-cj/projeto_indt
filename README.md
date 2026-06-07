# Plataforma de Seguros INDT

Plataforma de seguros com dois microserviços .NET 8 em **arquitetura hexagonal**, **PostgreSQL**, **RabbitMQ nativo** (`RabbitMQ.Client`) e **observabilidade OpenTelemetry + Serilog JSON**.

## Arquitetura

| Serviço | Papel |
|---------|-------|
| **PropostaService** (porta 5001) | Dono das regras de proposta — CRUD, alteração de status, publicação de eventos |
| **ContratacaoService** (porta 5002) | Handler assíncrono (fila `propostas.aprovadas`) + API de consultas e proxy HTTP para status |

Fluxo principal: criar proposta → aprovar (direto ou via proxy) → PropostaService publica evento → handler do ContratacaoService persiste contratação.

Documentação detalhada: [docs/arquitetura.md](docs/arquitetura.md) e [docs/observabilidade.md](docs/observabilidade.md).

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para Compose e testes de integração)

## Execução com Docker Compose

```bash
docker compose up --build
```

| Serviço | URL |
|---------|-----|
| PropostaService Swagger | http://localhost:5001/swagger |
| ContratacaoService Swagger | http://localhost:5002/swagger |
| RabbitMQ Management | http://localhost:15672 (guest/guest) |
| OTel Collector OTLP gRPC | localhost:4317 |

Health checks: `GET /health` e `GET /health/ready` em cada API.

## Execução local (sem Docker nas APIs)

1. Suba apenas a infraestrutura:

```bash
docker compose up postgres-proposta postgres-contratacao rabbitmq otel-collector -d
```

2. Execute as APIs:

```bash
dotnet run --project src/PropostaService/PropostaService.Api
dotnet run --project src/ContratacaoService/ContratacaoService.Api
```

As migrations EF Core são aplicadas automaticamente na inicialização.

## Testes

```bash
dotnet test INDT.Seguros.slnx
```

Testes de integração usam **Testcontainers** (Docker Desktop em execução). No Windows com Docker Desktop, configure o endpoint se necessário:

```bash
# PowerShell (Docker Desktop WSL2)
$env:DOCKER_HOST = "npipe:////./pipe/dockerDesktopLinuxEngine"
dotnet test INDT.Seguros.slnx
```

Arquivo `.testcontainers.properties` na raiz do repositório já aponta para o pipe correto.

## Exemplos curl

### 1. Criar proposta (PropostaService)

```bash
curl -X POST http://localhost:5001/api/propostas \
  -H "Content-Type: application/json" \
  -d '{"nomeSegurado":"Maria Silva","cpf":"12345678901","valorCobertura":15000}'
```

### 2. Aprovar via proxy (ContratacaoService)

```bash
curl -X PATCH http://localhost:5002/api/propostas/{PROPOSTA_ID}/status \
  -H "Content-Type: application/json" \
  -d '{"status":"Aprovada","observacao":null}'
```

### 3. Consultar contratações (após handler processar a fila)

```bash
curl "http://localhost:5002/api/contratacoes?page=1&pageSize=20"
curl http://localhost:5002/api/contratacoes/proposta/{PROPOSTA_ID}
```

### 4. Marcar pendência (observação obrigatória, 10–500 caracteres)

```bash
curl -X PATCH http://localhost:5001/api/propostas/{PROPOSTA_ID}/status \
  -H "Content-Type: application/json" \
  -d '{"status":"Pendencias","observacao":"Documentação incompleta — enviar comprovante de renda."}'
```

## Observabilidade

Logs estruturados JSON no stdout (Serilog). Traces e métricas exportados via OTLP para o collector configurado em `Observabilidade__EndpointOtlp`.

| Backend | Configuração |
|---------|--------------|
| **Datadog** | Apontar collector para agente Datadog OTLP ou usar exporter `datadog` |
| **ELK** | Exporter `elasticsearch` no OTel Collector ou coletar stdout JSON |
| **CloudWatch** | ADOT Collector com exporter AWS |

Detalhes: [docs/observabilidade.md](docs/observabilidade.md).

## Estrutura do repositório

```
src/
  PropostaService/          # Domain, Application, Infrastructure, Api
  ContratacaoService/       # Domain, Application, Infrastructure, Api
  Compartilhado/
    Compartilhado.Contratos/       # Eventos e constantes RabbitMQ
    Compartilhado.Observabilidade/ # OpenTelemetry + Serilog
tests/                      # Unitários e integração (Testcontainers)
deploy/                     # Configuração OTel Collector
docker-compose.yml
```

## RabbitMQ

- **Exchange:** `proposta.events` (topic)
- **Routing keys:** `proposta.aprovada`, `proposta.rejeitada`, `proposta.pendencias`
- **Fila consumida:** `propostas.aprovadas` (criada pelo handler do ContratacaoService)
