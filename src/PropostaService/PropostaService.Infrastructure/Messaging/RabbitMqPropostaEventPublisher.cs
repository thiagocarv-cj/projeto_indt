using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Ports;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using RabbitMQ.Client;
using Shared.Contracts.Eventos;
using Shared.Contracts.Mensageria;
using Shared.Observability.Messaging;

namespace PropostaService.Infrastructure.Messaging;

public class PublicadorEventosPropostaRabbitMq(
    IProvedorConexaoRabbitMq provedorConexao,
    ILogger<PublicadorEventosPropostaRabbitMq> logger) : IPublicadorEventosProposta
{
    private static readonly JsonSerializerOptions OpcoesJson = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public Task PublicarStatusAlteradoAsync(Proposta proposta, CancellationToken cancellationToken = default)
    {
        var (routingKey, body) = proposta.Status switch
        {
            StatusProposta.Aprovada => (
                ConstantesRabbitMq.ChaveRoteamentoAprovada,
                JsonSerializer.SerializeToUtf8Bytes(new EventoPropostaAprovada(
                    proposta.Id,
                    proposta.NomeSegurado,
                    proposta.Cpf,
                    proposta.ValorCobertura,
                    proposta.DataAtualizacao), OpcoesJson)),
            StatusProposta.Rejeitada => (
                ConstantesRabbitMq.ChaveRoteamentoRejeitada,
                JsonSerializer.SerializeToUtf8Bytes(new EventoPropostaRejeitada(
                    proposta.Id,
                    proposta.Observacao,
                    proposta.DataAtualizacao), OpcoesJson)),
            StatusProposta.Pendencias => (
                ConstantesRabbitMq.ChaveRoteamentoPendencias,
                JsonSerializer.SerializeToUtf8Bytes(new EventoPropostaPendente(
                    proposta.Id,
                    proposta.Observacao ?? string.Empty,
                    proposta.DataAtualizacao), OpcoesJson)),
            _ => throw new InvalidOperationException($"Status {proposta.Status} não gera evento.")
        };

        using var activity = Activity.Current?.Source.StartActivity("messaging.publish");
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", ConstantesRabbitMq.NomeExchange);
        activity?.SetTag("messaging.routing_key", routingKey);
        activity?.SetTag("proposta.id", proposta.Id.ToString());

        using var channel = provedorConexao.ObterConexao().CreateModel();
        channel.ExchangeDeclare(ConstantesRabbitMq.NomeExchange, ExchangeType.Topic, durable: true);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        PropagacaoRastreamentoRabbitMq.Injetar(Activity.Current, properties);

        channel.BasicPublish(
            ConstantesRabbitMq.NomeExchange,
            routingKey,
            basicProperties: properties,
            body: body);

        logger.LogInformation(
            "Evento publicado para proposta {PropostaId} com routing key {RoutingKey}",
            proposta.Id,
            routingKey);

        return Task.CompletedTask;
    }
}
