using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;

namespace Test.Mocks;

public class AdministratorServiceMock : IAdministratorService
{

    private static List<Administrator> administrators = new List<Administrator>(){
        new Administrator{
           Id = 1,
           Email = "adm@test.com",
           Password = "123456",
           Profile = "Adm"
        },
        new Administrator{
            Id = 2,
            Email = "editor@test.com",
            Password = "123456",
            Profile = "Editor"
        }
    };


    public Administrator Create(Administrator administrator)
    {
        administrator.Id = administrators.Count() + 1;
        administrators.Add(administrator);
        return administrator;
    }

    public List<Administrator> GetAll(int? page)
    {
        return administrators;
    }

    public Administrator? GetById(int id)
    {
        return administrators.Find(a => a.Id == id);

    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        return administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }
}