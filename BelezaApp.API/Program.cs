using DotNetEnv;
using Npgsql;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BelezaApp.API.Repositories;
using BelezaApp.API.Services;

// Configura o Dapper para mapear snake_case (banco) para PascalCase (C#)
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Ensina o Dapper a lidar com DateOnly e TimeOnly como parâmetros
SqlMapper.AddTypeHandler(new BelezaApp.API.TypeHandlers.DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new BelezaApp.API.TypeHandlers.TimeOnlyTypeHandler());

var builder = WebApplication.CreateBuilder(args);

Env.Load(".env");

// Configuração do JWT
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(Environment.GetEnvironmentVariable("DATABASE_CONNECTION"))
);

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();

builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();

builder.Services.AddScoped<IServicoRepository, ServicoRepository>();

builder.Services.AddScoped<IFuncionarioServicoRepository, FuncionarioServicoRepository>();

builder.Services.AddScoped<IAgendaRepository, AgendaRepository>();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IEnderecoService, EnderecoService>();

builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();

builder.Services.AddScoped<IServicoService, ServicoService>();

builder.Services.AddScoped<IFuncionarioServicoService, FuncionarioServicoService>();

builder.Services.AddScoped<IAgendaService, AgendaService>();

builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new BelezaApp.API.Converters.DateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new BelezaApp.API.Converters.DateOnlyConverter());
    options.JsonSerializerOptions.Converters.Add(new BelezaApp.API.Converters.TimeOnlyConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
