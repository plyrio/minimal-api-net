namespace MinimalAPI.Domain.ModelViews;

public struct ValidationError
{
    public List<string> Errors { get; set; }
}