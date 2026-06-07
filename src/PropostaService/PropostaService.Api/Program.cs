using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PropostaService.Application.Validadores;
using PropostaService.Infrastructure;
using PropostaService.Infrastructure.Messaging;
using PropostaService.Infrastructure.Persistence;
using Compartilhado.Observabilidade;

var builder = WebApplication.CreateBuilder(args);
builder.AdicionarObservabilidadeIndt("PropostaService");

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AdicionarInfraestrutura(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ValidadorSolicitacaoCriarProposta>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PropostaDb")!)
    .AddRabbitMQ(sp => sp.GetRequiredService<IProvedorConexaoRabbitMq>().ObterConexao());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.Run();

public partial class Program;
