using Serilog;
using API.PDV.Infra;
using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Npgsql;
using API.PDV.WebApi;
using API.PDV.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API PDV", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer. 
                        Exemplo: 'Bearer {token}'.
                        Endpoints sensíveis exigem autenticação e roles apropriadas (Admin, Gerente).",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Multitenancy: Schema context accessor
builder.Services.AddSingleton<ISchemaContextAccessor, SchemaContextAccessor>();

// Tenant repository
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

// Tenant migration service (para migração automática por tenant)
builder.Services.AddScoped<ITenantMigrationService, TenantMigrationService>();

// Product and Stock repositories and services
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<LoggingService>();

// Entity Framework Core configuration
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var schemaContextAccessor = serviceProvider.GetRequiredService<ISchemaContextAccessor>();
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Dapper configuration (IDbConnection injection)
builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection example for repositories/services
builder.Services.AddScoped(typeof(IRepository<>), typeof(API.PDV.Infra.GenericRepository<>));
builder.Services.AddScoped(typeof(IReadOnlyRepository<>), typeof(API.PDV.Infra.GenericRepository<>));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SENHA_FORTE_PADRAO";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "API.PDV";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "API.PDV.Client";

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Authorization with roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Operador", policy => policy.RequireRole("Operador", "Admin"));
});

// CORS restritivo
builder.Services.AddCors(options =>
{
    options.AddPolicy("Restrito", policy =>
    {
        policy.AllowAnyOrigin() // Ajuste conforme necessário
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Proteção contra ataques comuns (headers)
builder.Services.AddAntiforgery();
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

// Exception handling middleware
app.UseExceptionHandler("/error");

// HSTS e HTTPS
app.UseHsts();
app.UseHttpsRedirection();

// Middleware de proteção XSS e headers de segurança
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; object-src 'none'; frame-ancestors 'none'; base-uri 'self';");
    }
    else
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self';");
    }
    await next();
});

// CORS restritivo
app.UseCors("Restrito");

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Multitenancy: Tenant middleware
app.UseMiddleware<TenantMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Map controllers (expor endpoints reais)
app.MapControllers();

app.Run();
