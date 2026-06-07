using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ContratacaoService.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Eventos;
using Shared.Contracts.Mensageria;
using Shared.Observability.Messaging;

namespace ContratacaoService.Infrastructure.Messaging;

public class ConsumidorPropostaAprovadaPlanoFundo(
    IProvedorConexaoRabbitMq connectionProvider,
    IServiceScopeFactory scopeFactory,
    ILogger<ConsumidorPropostaAprovadaPlanoFundo> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions OpcoesJson = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private IModel? _channel;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = connectionProvider.ObterConexao();
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare(ConstantesRabbitMq.NomeExchange, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(ConstantesRabbitMq.FilaPropostasAprovadas, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(ConstantesRabbitMq.FilaPropostasAprovadas, ConstantesRabbitMq.NomeExchange, ConstantesRabbitMq.ChaveRoteamentoAprovada);
        _channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += AoReceberMensagemAsync;

        _channel.BasicConsume(ConstantesRabbitMq.FilaPropostasAprovadas, autoAck: false, consumer);
        logger.LogInformation("Consumer iniciado na fila {Queue}", ConstantesRabbitMq.FilaPropostasAprovadas);

        stoppingToken.Register(() =>
        {
            _channel?.Close();
            _channel?.Dispose();
        });

        return Task.CompletedTask;
    }

    private async Task AoReceberMensagemAsync(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel is null)
            return;

        using var activity = Activity.Current?.Source.StartActivity("messaging.consume");
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", ConstantesRabbitMq.FilaPropostasAprovadas);

        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var evento = JsonSerializer.Deserialize<EventoPropostaAprovada>(json, OpcoesJson)
                ?? throw new InvalidOperationException("Payload inválido.");

            activity?.SetTag("proposta.id", evento.PropostaId.ToString());

            using var scope = scopeFactory.CreateScope();
            var casoDeUso = scope.ServiceProvider.GetRequiredService<IIngerirPropostaAprovadaCasoDeUso>();
            await casoDeUso.ExecutarAsync(evento);

            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar mensagem da fila {Queue}", ConstantesRabbitMq.FilaPropostasAprovadas);
            _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}
