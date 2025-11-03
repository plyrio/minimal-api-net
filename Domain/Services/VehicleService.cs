using MinimalAPI.Domain.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Infrastructure.Db;

namespace MinimalAPI.Domain.Services;

public class VehicleService : IVehicleService
{

    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context)
    {
        _context = context;
    }

    public void Create(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public List<Vehicle> GetAll(int page = 1, string? name = null, string? brand = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => v.Name.ToLower().Contains(name));

        }

        int itemsPerPage = 10;

        query = query.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);

        return query.ToList();


    }

    public Vehicle? GetById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}