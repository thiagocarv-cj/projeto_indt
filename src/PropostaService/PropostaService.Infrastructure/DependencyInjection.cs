using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.Handlers;
using PropostaService.Application.Ports;
using PropostaService.Application.UseCases;
using PropostaService.Infrastructure.Messaging;
using PropostaService.Infrastructure.Persistence;

namespace PropostaService.Infrastructure;

public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcoesRabbitMq>(configuration.GetSection(OpcoesRabbitMq.SectionName));

        services.AddDbContext<PropostaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PropostaDb")));

        services.AddSingleton<IProvedorConexaoRabbitMq, ProvedorConexaoRabbitMq>();
        services.AddScoped<IRepositorioProposta, RepositorioProposta>();
        services.AddScoped<IPublicadorEventosProposta, PublicadorEventosPropostaRabbitMq>();

        services.AddScoped<ICriarPropostaCasoDeUso, CriarPropostaManipulador>();
        services.AddScoped<IObterPropostaCasoDeUso, ObterPropostaManipulador>();
        services.AddScoped<IConsultarStatusPropostaCasoDeUso, ConsultarStatusPropostaManipulador>();
        services.AddScoped<IListarPropostasCasoDeUso, ListarPropostasManipulador>();
        services.AddScoped<IAlterarStatusPropostaCasoDeUso, AlterarStatusPropostaManipulador>();

        return services;
    }
}
