using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.Portas.Entrada;
using PropostaService.Application.Portas.Saida;
using PropostaService.Application.CasosDeUso;
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

        services.AddScoped<ICriarPropostaCasoDeUso, CriarPropostaCasoDeUso>();
        services.AddScoped<IObterPropostaCasoDeUso, ObterPropostaCasoDeUso>();
        services.AddScoped<IConsultarStatusPropostaCasoDeUso, ConsultarStatusPropostaCasoDeUso>();
        services.AddScoped<IListarPropostasCasoDeUso, ListarPropostasCasoDeUso>();
        services.AddScoped<IAlterarStatusPropostaCasoDeUso, AlterarStatusPropostaCasoDeUso>();

        return services;
    }
}
