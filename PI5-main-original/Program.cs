using System;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pi5.database;
using pi5.Interfaces.Services;
using pi5.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAcoesService, AcoesService>();
builder.Services.AddScoped<IIntegracaoService, IntegracaoService>();
//Injeção de dependencia.

// Configurando o banco de dados
var connectionString = builder.Configuration.GetConnectionString("ConexaoBanco");

builder.Services.AddDbContext<PI5Context>(opts => opts.UseSqlServer(connectionString));

var app = builder.Build();

// Configurando a rotina agendada para executar às 3:00 AM
var now = DateTime.Now;
var startOfDay = now.Date.AddHours(3);
var initialDelay = startOfDay > now ? startOfDay - now : TimeSpan.FromHours(24) - (now - startOfDay);
var timer = new Timer(async _ =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var acoesService = services.GetRequiredService<IAcoesService>();

        await acoesService.AtualizaDados(); // Correção aqui
        await acoesService.HistoricoFechamento();
    }
}, null, initialDelay, TimeSpan.FromHours(24)); // Executando a cada 24 horas

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
