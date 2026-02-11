using Cortex.Mediator.DependencyInjection;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TaskTracker.API.Application.Common.Interfaces;
using TaskTracker.API.Infrastructure.Data;
using TaskTracker.API.Infrastructure.Repositories;
using TaskTracker.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// EF Core In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskTrackerDb"));

// Repository
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Cortex Mediator
builder.Services.AddCortexMediator(
    builder.Configuration,
    [typeof(Program)]);

// Mapster
TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
