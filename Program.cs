using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

DotNetEnv.Env.Load();

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    )
);

var app = builder.Build();
#endregion

#region  Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
}).WithTags("Administrators");
#endregion


#region Vehicle

app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Age = vehicleDTO.Age
    };
    vehicleService.Create(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicle", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAll(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicle/{id}", (int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Age = vehicleDTO.Age;

    vehicleService.Update(vehicle);


    return Results.Ok(vehicle);
}).WithTags("Vehicles");


app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);


    return Results.NoContent();
}).WithTags("Vehicles");


#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();


app.Run();
#endregion