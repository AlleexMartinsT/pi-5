using Microsoft.EntityFrameworkCore;
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
