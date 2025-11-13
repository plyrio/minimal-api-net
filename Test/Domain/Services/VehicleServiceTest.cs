using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infrastructure.Db;


namespace Test.Domain.Services;

[TestClass]
public class VehicleServiceTest
{

    private AppDbContext CreateTestContext()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath! ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
        .SetBasePath(path ?? Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
        var configuration = builder.Build();

        return new AppDbContext(configuration);
    }


    [TestMethod]
    public void TestSaveVehicle()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles");

        var vehicle = new Vehicle();
        vehicle.Name = "Test Vehicle";
        vehicle.Brand = "Test Brand";
        vehicle.Age = 2020;

        var vehicleService = new VehicleService(context);

        //Act
        vehicleService.Create(vehicle);

        //Assert
        Assert.AreEqual(1, vehicleService.GetAll(1).Count());

    }

    [TestMethod]
    public void TestGetVehicleById()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles");

        var vehicle = new Vehicle();
        vehicle.Name = "Test Vehicle";
        vehicle.Brand = "Test Brand";
        vehicle.Age = 2020;

        var vehicleService = new VehicleService(context);

        //Act
        vehicleService.Create(vehicle);
        var vehicle1 = vehicleService.GetById(vehicle.Id);

        //Assert
        Assert.AreEqual(1, vehicle1?.Id);

    }

    [TestMethod]

    public void TestGetAllVehicles()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles");

        var vehicle = new Vehicle();
        vehicle.Name = "Test Vehicle";
        vehicle.Brand = "Test Brand";
        vehicle.Age = 2020;

        var vehicle1 = new Vehicle();
        vehicle1.Name = "Test Vehicle";
        vehicle1.Brand = "Test Brand";
        vehicle1.Age = 2020;

        var vehicleService = new VehicleService(context);

        //Act
        vehicleService.Create(vehicle);
        vehicleService.Create(vehicle1);
        vehicleService.GetAll(1);

        //Assert
        Assert.AreEqual(2, vehicleService.GetAll(1).Count());
    }

    [TestMethod]
    public void TestDeleteVehicle()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles");

        var vehicle = new Vehicle();
        vehicle.Name = "Test Vehicle";
        vehicle.Brand = "Test Brand";
        vehicle.Age = 2020;

        var vehicle1 = new Vehicle();
        vehicle1.Name = "Test Vehicle";
        vehicle1.Brand = "Test Brand";
        vehicle1.Age = 2020;

        var vehicleService = new VehicleService(context);

        //Act
        vehicleService.Create(vehicle);
        vehicleService.Create(vehicle1);    
        vehicleService.Delete(vehicle);
    

        //Assert
        Assert.AreEqual(1, vehicleService.GetAll(1).Count());
    }

    [TestMethod]
    public void TestUpdateVehicle()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles");

        var vehicle = new Vehicle();
        vehicle.Name = "Test Vehicle";
        vehicle.Brand = "Test Brand";
        vehicle.Age = 2020;

        var vehicleService = new VehicleService(context);

        //Act
        vehicleService.Create(vehicle);
        vehicle.Name = "Updated Vehicle";
        vehicleService.Update(vehicle);

        //Assert
        Assert.AreEqual("Updated Vehicle", vehicleService.GetById(vehicle.Id)?.Name);

    }
}