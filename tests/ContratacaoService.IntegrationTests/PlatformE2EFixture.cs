extern alias PropostaApi;

using ContratacaoService.Application.Ports;
using ContratacaoService.Infrastructure.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace ContratacaoService.IntegrationTests;

public class AmbienteTesteE2EPlataforma : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresProposta;
    private PostgreSqlContainer? _postgresContratacao;
    private RabbitMqContainer? _rabbit;

    public WebApplicationFactory<PropostaApi::Program>? FabricaProposta { get; private set; }
    public WebApplicationFactory<Program>? FabricaContratacao { get; private set; }
    public bool EstaPronto => FabricaProposta is not null && FabricaContratacao is not null;
    public HttpClient ClienteProposta => FabricaProposta!.CreateClient();
    public HttpClient ClienteContratacao => FabricaContratacao!.CreateClient();

    public async Task InitializeAsync()
    {
        try
        {
            _postgresProposta = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("proposta_db")
                .WithUsername("proposta")
                .WithPassword("proposta123")
                .Build();

            _postgresContratacao = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("contratacao_db")
                .WithUsername("contratacao")
                .WithPassword("contratacao123")
                .Build();

            _rabbit = new RabbitMqBuilder("rabbitmq:3.13-management-alpine")
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            await Task.WhenAll(
                _postgresProposta.StartAsync(),
                _postgresContratacao.StartAsync(),
                _rabbit.StartAsync());

            FabricaProposta = new WebApplicationFactory<PropostaApi::Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:PropostaDb", _postgresProposta.GetConnectionString());
                builder.UseSetting("RabbitMQ:Host", _rabbit.Hostname);
                builder.UseSetting("RabbitMQ:Port", _rabbit.GetMappedPublicPort(5672).ToString());
                builder.UseSetting("RabbitMQ:UserName", "guest");
                builder.UseSetting("RabbitMQ:Password", "guest");
                builder.UseSetting("Observability:OtlpEndpoint", string.Empty);
            });

            var urlBaseProposta = FabricaProposta.Server.BaseAddress!.ToString().TrimEnd('/');

            FabricaContratacao = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:ContratacaoDb", _postgresContratacao.GetConnectionString());
                builder.UseSetting("RabbitMQ:Host", _rabbit.Hostname);
                builder.UseSetting("RabbitMQ:Port", _rabbit.GetMappedPublicPort(5672).ToString());
                builder.UseSetting("RabbitMQ:UserName", "guest");
                builder.UseSetting("RabbitMQ:Password", "guest");
                builder.UseSetting("PropostaService:BaseUrl", urlBaseProposta);
                builder.UseSetting("Observability:OtlpEndpoint", string.Empty);

                builder.ConfigureTestServices(services =>
                {
                    services.AddHttpClient<IClienteServicoProposta, ClienteHttpProposta>()
                        .ConfigurePrimaryHttpMessageHandler(_ => FabricaProposta!.Server.CreateHandler());
                });
            });
        }
        catch (Exception)
        {
            FabricaProposta = null;
            FabricaContratacao = null;
        }
    }

    public async Task DisposeAsync()
    {
        if (FabricaContratacao is not null)
            await FabricaContratacao.DisposeAsync();
        if (FabricaProposta is not null)
            await FabricaProposta.DisposeAsync();

        if (_postgresProposta is not null)
            await _postgresProposta.DisposeAsync();
        if (_postgresContratacao is not null)
            await _postgresContratacao.DisposeAsync();
        if (_rabbit is not null)
            await _rabbit.DisposeAsync();
    }
}
