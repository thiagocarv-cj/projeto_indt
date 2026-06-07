using ContratacaoService.Infrastructure.Messaging;

using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ContratacaoService.Infrastructure.Messaging;

public class OpcoesRabbitMq
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
}

public interface IProvedorConexaoRabbitMq
{
    IConnection ObterConexao();
}

public sealed class ProvedorConexaoRabbitMq : IProvedorConexaoRabbitMq, IDisposable
{
    private readonly Lazy<IConnection> _connection;

    public ProvedorConexaoRabbitMq(Microsoft.Extensions.Options.IOptions<OpcoesRabbitMq> options)
    {
        _connection = new Lazy<IConnection>(() =>
        {
            var cfg = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = cfg.Host,
                Port = cfg.Port,
                UserName = cfg.UserName,
                Password = cfg.Password,
                VirtualHost = cfg.VirtualHost,
                DispatchConsumersAsync = true
            };
            return factory.CreateConnection();
        });
    }

    public IConnection ObterConexao() => _connection.Value;

    public void Dispose()
    {
        if (_connection.IsValueCreated)
            _connection.Value.Dispose();
    }
}
