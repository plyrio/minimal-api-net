namespace MinimalAPI.Domain.DTOs;

public record VehicleDTO
{
    public string Name { get; set; } = default!;
    public string Brand { get; set; } = default!;
    public int Age { get; set; } = default!;
}