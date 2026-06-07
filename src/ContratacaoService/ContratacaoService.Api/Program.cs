using System.Text.Json.Serialization;
using ContratacaoService.Application.Ports;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Infrastructure;
using ContratacaoService.Infrastructure.Messaging;
using ContratacaoService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Propostas;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);
builder.AdicionarObservabilidadeIndt("ContratacaoService");

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AdicionarInfraestrutura(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("ContratacaoDb")!)
    .AddRabbitMQ(sp => sp.GetRequiredService<IProvedorConexaoRabbitMq>().ObterConexao());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();
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
