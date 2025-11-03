using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Infrastructure.Db;

namespace MinimalAPI.Domain.Services;

public class AdministratorService : IAdministratorService
{

    private readonly AppDbContext _context;

    public AdministratorService(AppDbContext context)
    {
        _context = context;
    }


    public Administrator? Login(LoginDTO loginDTO)
    {
        var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();

        return adm;
    }
}