using MinimalAPI.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var vehicle = new Vehicle();

        // Act
        vehicle.Id = 1;
        vehicle.Brand = "Toyota";
        vehicle.Name = "Corolla";
        vehicle.Age = 2020;

        // Assert
        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Toyota", vehicle.Brand);
        Assert.AreEqual("Corolla", vehicle.Name);
        Assert.AreEqual(2020, vehicle.Age);
    }
}