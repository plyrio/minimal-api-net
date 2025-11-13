using System.Net;
using System.Text;
using System.Text.Json;
using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.ModelViews;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdministratorTestRequest
{

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }


    [TestMethod]
    public async Task TestGetSetProperties()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@test.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        //Act
        var response = await Setup.client.PostAsync("administrators/login", content);


        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        
        Assert.IsNotNull(admLogged?.Email ?? "");
        Assert.IsNotNull(admLogged?.Profile ?? "");
        Assert.IsNotNull(admLogged?.Token ?? "");

        Console.WriteLine(admLogged?.Token);
      

    }
}