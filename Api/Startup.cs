using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPI;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Enums;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infrastructure.Db;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
            Configuration = configuration;
            key = Configuration.GetSection("Jwt")?.ToString() ?? "";
                
    }

    private string key = "";

    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

        services.AddAuthorization();

        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehicleService, VehicleService>();


        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "Jwt",
                In = ParameterLocation.Header,
                Description = "Insert the JWT token like: Bearer {your token}"

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

        }, new string[] {}

    }
            });


        });

        services.AddDbContext<AppDbContext>(
            options => options.UseMySql(
                Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
            )
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            #region  Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().AllowAnonymous().WithTags("Home");
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
                    new Claim("Profile", administrator.Profile),
                    new Claim(ClaimTypes.Role, administrator.Profile)
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }


            endpoints.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
            {
                var adm = administratorService.Login(loginDTO);
                if (adm != null)
                {
                    string token = GenerateJwtToken(adm);
                    return Results.Ok(new AdministratorLogged
                    {
                        Email = adm.Email,
                        Profile = adm.Profile,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).WithTags("Administrators");

            endpoints.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
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


            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,adm" }).WithTags("Administrators");


            endpoints.MapGet("/administrator", ([FromQuery] int? page, IAdministratorService administratorService) =>
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,adm" }).WithTags("Administrators");

            endpoints.MapGet("/administrator/{id}", (int id, IAdministratorService administratorService) =>
            {
                var administrator = administratorService.GetById(id);
                if (administrator == null) return Results.NotFound();
                return Results.Ok(new AdministratorModelView
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile
                });
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administrators");



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

            endpoints.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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

            endpoints.MapGet("/vehicle", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.GetAll(page);

                return Results.Ok(vehicles);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Vehicles");

            endpoints.MapGet("/vehicle/{id}", (int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);
                if (vehicle == null) return Results.NotFound();
                return Results.Ok(vehicle);
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

            endpoints.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");


            endpoints.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);
                if (vehicle == null) return Results.NotFound();

                vehicleService.Delete(vehicle);


                return Results.NoContent();
            }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");


            #endregion

        });
    }
}