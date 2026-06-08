using System.Text.Json.Serialization;
using ContratacaoService.Api.Documentacao;
using ContratacaoService.Application.Excecoes;
using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Infrastructure;
using ContratacaoService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Compartilhado.Contratos.Propostas;
using Compartilhado.Observabilidade;

var builder = WebApplication.CreateBuilder(args);
builder.AdicionarObservabilidadeIndt("ContratacaoService");

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AdicionarDocumentacaoSwaggerContratacao();
builder.Services.AdicionarInfraestrutura(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("ContratacaoDb")!);

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

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.Run();

public partial class Program;
