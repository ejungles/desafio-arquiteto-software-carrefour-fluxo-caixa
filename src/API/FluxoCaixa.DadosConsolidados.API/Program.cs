using AspNetCoreRateLimit;
using FluxoCaixa.Application.Handlers;
using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Mapping;
using FluxoCaixa.Application.Services;
using FluxoCaixa.DadosConsolidados.API.Services;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Infra;
using FluxoCaixa.Infra.Caching;
using FluxoCaixa.Infra.DataContext;
using FluxoCaixa.Infra.Repositories;
using FluxoCaixa.Shared.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações de Infraestrutura
builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

builder.Services.AddSingleton<MongoDbContext>();

// 2. Configuração do Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// 3. Registro de Repositórios
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
builder.Services.AddScoped<IConsolidadoRepository, ConsolidadoRepository>();
builder.Services.AddScoped<IProcessamentoRepository, ProcessamentoRepository>();

// 4. Registro de Serviços de Domínio
builder.Services.AddScoped<IConsolidadoService, ConsolidadoService>();
builder.Services.AddScoped<ILancamentoCriadoHandler, LancamentoCriadoEventHandler>();

// CORREÇÃO: Adicionar o registro do IUnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// CORREÇÃO: Adicionar o AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 5. Configuração do MediatR (SOMENTE ESTE REGISTRO)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ConsolidadoQueryHandler).Assembly);
    cfg.Lifetime = ServiceLifetime.Scoped;
});

// 6. Configuração do Consumer RabbitMQ
builder.Services.AddHostedService<LancamentoCriadoConsumerService>();

// 7. Configuração dos Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 8. Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Dados Consolidados",
        Version = "v1",
        Description = "Consulta e geração de relatórios de saldos consolidados",
        Contact = new OpenApiContact
        {
            Name = "Suporte Técnico",
            Email = "suporte@fluxocaixa.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 9. Configuração de Autenticação JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// 10. Configuração de Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireConsolidadoAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// 11. Configuração de Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// 12. Configuração de Health Checks
builder.Services
    .AddSingleton(sp => new MongoClient(builder.Configuration.GetConnectionString("MongoDB")))
    .AddHealthChecks()
    .AddMongoDb(databaseNameFactory: sp => "theName")
    .AddRedis(builder.Configuration.GetConnectionString("Redis"), name: "redis")
    .AddRabbitMQ(sp => CreateConnection(), name: "rabbitmq");

var app = builder.Build();

// 13. Configuração do Pipeline HTTP
app.UseIpRateLimiting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dados Consolidados v1");
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// 14. Método auxiliar para conexão RabbitMQ
static Task<IConnection> CreateConnection()
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri("amqps://user:pass@host/vhost"),
    };
    return factory.CreateConnectionAsync();
}