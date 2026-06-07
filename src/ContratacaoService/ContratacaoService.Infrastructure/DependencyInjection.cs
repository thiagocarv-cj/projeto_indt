using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;
using ContratacaoService.Application.CasosDeUso;
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
        services.AddDbContext<ContratacaoDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("ContratacaoDb")));

        services.Configure<OpcoesRabbitMq>(configuration.GetSection(OpcoesRabbitMq.SectionName));
        services.Configure<OpcoesServicoProposta>(configuration.GetSection(OpcoesServicoProposta.SectionName));

        services.AddSingleton<IProvedorConexaoRabbitMq, ProvedorConexaoRabbitMq>();
        services.AddScoped<IRepositorioContratacao, RepositorioContratacao>();
        services.AddHostedService<ConsumidorPropostaAprovadaPlanoFundo>();
        services.AddHttpClient<IClienteServicoProposta, ClienteHttpProposta>();

        services.AddScoped<IIngerirPropostaAprovadaCasoDeUso, IngerirPropostaAprovadaCasoDeUso>();
        services.AddScoped<IConsultarStatusPropostaIntermediarioCasoDeUso, ConsultarStatusPropostaIntermediarioCasoDeUso>();
        services.AddScoped<IAlterarStatusPropostaIntermediarioCasoDeUso, AlterarStatusPropostaIntermediarioCasoDeUso>();
        services.AddScoped<IListarContratacoesCasoDeUso, ListarContratacoesCasoDeUso>();
        services.AddScoped<IObterContratacaoCasoDeUso, ObterContratacaoCasoDeUso>();
        services.AddScoped<IObterContratacaoPorPropostaCasoDeUso, ObterContratacaoPorPropostaCasoDeUso>();

        return services;
    }
}
