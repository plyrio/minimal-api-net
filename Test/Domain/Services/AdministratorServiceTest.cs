using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Services;
using MinimalAPI.Infrastructure.Db;


namespace Test.Domain.Services;

[TestClass]
public class AdministratorServiceTest
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
    public void TestSaveAdministrator()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator();
        adm.Email = "test3@test.com";
        adm.Password = "12456";
        adm.Profile = "Adm";

        var adiministratorService = new AdministratorService(context);

        //Act
        adiministratorService.Create(adm);

        //Assert
        Assert.AreEqual(1, adiministratorService.GetAll(1).Count());

    }

    [TestMethod]
    public void TestGetAdministratorById()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator();
        adm.Email = "test3@test.com";
        adm.Password = "12456";
        adm.Profile = "Adm";

        var adiministratorService = new AdministratorService(context);

        //Act
        adiministratorService.Create(adm);
        var administrator = adiministratorService.GetById(adm.Id);

        //Assert
        Assert.AreEqual(1, administrator?.Id);

    }

    [TestMethod]

    public void TestGetAllAdministrators()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator();
        adm.Email = "test4@test.com";
        adm.Password = "12456";
        adm.Profile = "Adm";

        var adiministratorService = new AdministratorService(context);

        // Act
        adiministratorService.Create(adm);
        adiministratorService.GetAll(1);

        //Assert
        Assert.AreEqual(1, adiministratorService.GetAll(1).Count());
    }
}