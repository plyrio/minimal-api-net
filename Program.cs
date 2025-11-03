using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

DotNetEnv.Env.Load();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    )
);

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();


app.Run();

