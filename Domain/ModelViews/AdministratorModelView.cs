using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Enums;

namespace MinimalAPI.Domain.ModelViews;

public record AdministratorModelView
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;

    public string Profile { get; set; } = default!;
}