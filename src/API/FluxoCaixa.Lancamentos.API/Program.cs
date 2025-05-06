using FluentValidation;
using FluxoCaixa.Application.Handlers;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Services;
using FluxoCaixa.Application.Validations;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Infra.Caching;
using FluxoCaixa.Infra.DataContext;
using FluxoCaixa.Infra.Messaging;
using FluxoCaixa.Infra.Repositories;
using FluxoCaixa.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

/*------------------------------------------------------------
                CONFIGURAÇÃO DE SERVIÇOS
------------------------------------------------------------*/

// Configuração do Entity Framework Core para SQL Server
builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

builder.Services.AddSingleton<MongoDbContext>();

// Configuração do Redis para cache
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Configuração do RabbitMQ
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);
builder.Services.AddHostedService<RabbitMQConsumer>();

builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
//builder.Services.AddSingleton<RabbitMQPublisher>();

builder.Services.AddScoped<ILancamentoCriadoHandler, LancamentoCriadoEventHandler>();
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();

// Injeção de dependências
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
builder.Services.AddScoped<IUnitOfWork>(provider =>
    provider.GetRequiredService<SqlServerDbContext>());

builder.Services.AddScoped<IConsolidadoService, ConsolidadoService>();
builder.Services.AddScoped<IConsolidadoRepository, ConsolidadoRepository>();

// Validações
builder.Services.AddValidatorsFromAssemblyContaining<CriarLancamentoValidator>();

// Configuração de resiliência com Polly
builder.Services.AddHttpClient("ResilientHttpClient")
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// Configuração da API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "API de Lançamentos Financeiros",
        Version = "v1",
        Description = "API responsável pelo gerenciamento de transações financeiras",
        Contact = new()
        {
            Name = "Suporte Técnico",
            Email = "suporte@fluxocaixa.com"
        }
    });
});

var app = builder.Build();

/*------------------------------------------------------------
                POLÍTICAS DE RESILIÊNCIA
------------------------------------------------------------*/

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

/*------------------------------------------------------------
                CONFIGURAÇÃO DO PIPELINE
------------------------------------------------------------*/

app.UseHttpsRedirection();

// Aplicar migrações automáticas
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>();
    if (!dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.EnsureCreated();
    }
    else
    {
        dbContext.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lançamentos v1");
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.ConfigObject.AdditionalItems["syntaxHighlight"] = new { activated = false };
    });
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();