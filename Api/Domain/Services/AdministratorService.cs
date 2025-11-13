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

    public Administrator Create(Administrator administrator)
    {
        _context.Administrators.Add(administrator);
        _context.SaveChanges();
        return administrator;
    }

    public Administrator? GetById(int id)
    {
        return _context.Administrators.Where(v => v.Id == id).FirstOrDefault();
    }

    public List<Administrator> GetAll(int? page)
    {
        var query = _context.Administrators.AsQueryable();

        int itemsPerPage = 10;

        if (page != null)
        {
            query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
        }

        return query.ToList();

    }
}