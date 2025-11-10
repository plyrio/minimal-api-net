using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


DotNetEnv.Env.Load();

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

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

string GenerateJwtToken(Administrator administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim("Email", administrator.Email),
new Claim("Profile", administrator.Profile)
    };

    var token = new JwtSecurityToken(
claims: claims,
expires: DateTime.Now.AddDays(1),
signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}


app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var adm = administratorService.Login(loginDTO);
    if (adm != null)
    {
        string token = GenerateJwtToken(adm);
        return Results.Ok(new LoggedAdministrator
        {
            Email = adm.Email,
            Profile = adm.Profile,
Token = token
        });
}
    else{
        return Results.Unauthorized();
    }
}).WithTags("Administrators");

app.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validation = new ValidationError
    {
        Errors = new List<string>()
    };

    if (string.IsNullOrEmpty(administratorDTO.Email))
    {
        validation.Errors.Add("Email is required. Cannot be null or empty.");
    }

if (string.IsNullOrEmpty(administratorDTO.Password))
{
    validation.Errors.Add("Password is required. Cannot be null or empty.");
}
    
    if (administratorDTO.Profile == null)
    {
        validation.Errors.Add("Profile is required. Cannot be empty.");
    }

    if (validation.Errors.Count > 0)
    {
    return Results.BadRequest(validation);
    }

    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Profile = administratorDTO.Profile?.ToString() ?? Profile.Editor.ToString()
    };

    administratorService.Create(administrator);

    return Results.Created($"/administrator/{administrator.Id}", new AdministratorModelView
        {
            Id = administrator.Id,
            Email = administrator.Email,
            Profile = administrator.Profile
        });
    

}).RequireAuthorization().WithTags("Administrators");


app.MapGet("/administrator", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorModelView>();
    var administrators = administratorService.GetAll(page);

    foreach (var administrator in administrators)
    {
        adms.Add(new AdministratorModelView
        {
            Id = administrator.Id,
            Email = administrator.Email,
            Profile = administrator.Profile
        });
    }

    return Results.Ok(adms);
}).RequireAuthorization().WithTags("Administrators");

app.MapGet("/administrator/{id}", (int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetById(id);
    if (administrator == null) return Results.NotFound();
    return Results.Ok(new AdministratorModelView
        {
            Id = administrator.Id,
            Email = administrator.Email,
            Profile = administrator.Profile
        });
}).RequireAuthorization().WithTags("Administrators");

        

#endregion


#region Vehicle

ValidationError validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationError
    {
        Errors = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name))
    {
        validation.Errors.Add("Name is required. Cannot be null or empty.");
    }

    if (string.IsNullOrEmpty(vehicleDTO.Brand))
    {
        validation.Errors.Add("meme is required. Cannot be null or empty.");
    }

    if (vehicleDTO.Age < 1950 || vehicleDTO.Age > DateTime.Now.Year)
    {
        validation.Errors.Add("Age cannot be less than 1950 or greater than the current year.");
    }
    return validation;
}

app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{

    var validation = validationDTO(vehicleDTO);
    if (validation.Errors.Count > 0)
    {
        return Results.BadRequest(validation);
    }



    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Age = vehicleDTO.Age
    };
    vehicleService.Create(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicle", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAll(page);

    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicle/{id}", (int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");
            
app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{

    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();
        

    var validation = validationDTO(vehicleDTO);
    if (validation.Errors.Count > 0)
    {
        return Results.BadRequest(validation);
    }



    
    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Age = vehicleDTO.Age;

    vehicleService.Update(vehicle);


    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");


app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);


    return Results.NoContent();
}).RequireAuthorization().WithTags("Vehicles");


#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization(); 

app.Run();
#endregion