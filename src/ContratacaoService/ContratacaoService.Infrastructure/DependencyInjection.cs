using ContratacaoService.Application.Handlers;
using ContratacaoService.Application.Ports;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Infrastructure.Http;
using ContratacaoService.Infrastructure.Messaging;
using ContratacaoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContratacaoService.Infrastructure;

public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcoesRabbitMq>(configuration.GetSection(OpcoesRabbitMq.SectionName));
        services.Configure<OpcoesServicoProposta>(configuration.GetSection(OpcoesServicoProposta.SectionName));

        services.AddDbContext<ContratacaoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ContratacaoDb")));

        services.AddSingleton<IProvedorConexaoRabbitMq, ProvedorConexaoRabbitMq>();
        services.AddScoped<IRepositorioContratacao, RepositorioContratacao>();

        services.AddHttpClient<IClienteServicoProposta, ClienteHttpProposta>();

        services.AddScoped<IIngerirPropostaAprovadaCasoDeUso, IngerirPropostaAprovadaManipulador>();
        services.AddScoped<IConsultarStatusPropostaIntermediarioCasoDeUso, ConsultarStatusPropostaIntermediarioManipulador>();
        services.AddScoped<IAlterarStatusPropostaIntermediarioCasoDeUso, AlterarStatusPropostaIntermediarioManipulador>();
        services.AddScoped<IListarContratacoesCasoDeUso, ListarContratacoesManipulador>();
        services.AddScoped<IObterContratacaoCasoDeUso, ObterContratacaoManipulador>();
        services.AddScoped<IObterContratacaoPorPropostaCasoDeUso, ObterContratacaoPorPropostaManipulador>();

        services.AddHostedService<ConsumidorPropostaAprovadaPlanoFundo>();

        return services;
    }
}
