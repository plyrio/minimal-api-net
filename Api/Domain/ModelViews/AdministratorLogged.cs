using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Enums;

namespace MinimalAPI.Domain.ModelViews;

public record AdministratorLogged
{
    public string Email { get; set; } = default!;

    public string Profile { get; set; } = default!;

    public string Token { get; set; } = default!;
}