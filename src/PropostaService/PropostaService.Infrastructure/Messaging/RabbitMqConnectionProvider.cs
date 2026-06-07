using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PropostaService.Infrastructure.Messaging;

public interface IProvedorConexaoRabbitMq
{
    IConnection ObterConexao();
}

public sealed class ProvedorConexaoRabbitMq : IProvedorConexaoRabbitMq, IDisposable
{
    private readonly Lazy<IConnection> _connection;

    public ProvedorConexaoRabbitMq(IOptions<OpcoesRabbitMq> options)
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
