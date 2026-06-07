using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace PropostaService.IntegrationTests;

public class AmbienteTesteApiProposta : IAsyncLifetime
{
    private PostgreSqlContainer? _postgres;
    private RabbitMqContainer? _rabbit;

    public WebApplicationFactory<Program>? Fabrica { get; private set; }
    public bool EstaPronto => Fabrica is not null;
    public string MotivoIndisponibilidade { get; private set; } = "Docker indisponível — inicie o Docker Desktop para executar testes de integração.";
    public HttpClient Cliente => Fabrica!.CreateClient();

    public async Task InitializeAsync()
    {
        try
        {
            _postgres = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("proposta_db")
                .WithUsername("proposta")
                .WithPassword("proposta123")
                .Build();

            _rabbit = new RabbitMqBuilder("rabbitmq:3.13-management-alpine")
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            await Task.WhenAll(_postgres.StartAsync(), _rabbit.StartAsync());

            Fabrica = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:PropostaDb", _postgres.GetConnectionString());
                builder.UseSetting("RabbitMQ:Host", _rabbit.Hostname);
                builder.UseSetting("RabbitMQ:Port", _rabbit.GetMappedPublicPort(5672).ToString());
                builder.UseSetting("RabbitMQ:UserName", "guest");
                builder.UseSetting("RabbitMQ:Password", "guest");
                builder.UseSetting("Observabilidade:EndpointOtlp", string.Empty);
            });
        }
        catch (Exception ex)
        {
            Fabrica = null;
            MotivoIndisponibilidade = ex.Message;
        }
    }

    public async Task DisposeAsync()
    {
        if (Fabrica is not null)
            await Fabrica.DisposeAsync();
        if (_postgres is not null)
            await _postgres.DisposeAsync();
        if (_rabbit is not null)
            await _rabbit.DisposeAsync();
    }
}
