using System.Text;
using System.Text.Json.Serialization;
using Cortex.Mediator.DependencyInjection;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Application.Common.Models;
using TaskTracker.API.Infrastructure.Auth;
using TaskTracker.API.Infrastructure.Data;
using TaskTracker.API.Infrastructure.Repositories;
using TaskTracker.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// EF Core In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskTrackerDb"));

// Repository
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Cortex Mediator
builder.Services.AddCortexMediator(
    builder.Configuration,
    [typeof(Program)]);

// Mapster
TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Password Hasher
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// JWT
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName);
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

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
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Problem Details
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Tracker API",
        Version = "v1",
        Description = "A Task Management API built with Clean Architecture and CQRS pattern."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Tracker API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
