using MinimalAPI.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var adm = new Administrator();

        //Act
        adm.Email = "test@test.com";
        adm.Password = "12456";
        adm.Profile = "Adm";
        adm.Id = 1;

        //Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("12456", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);

    }
}