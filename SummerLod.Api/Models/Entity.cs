namespace SummerLod.Api.Models;

public class Entity
{
    public List<string> Ids { get; set; } = [];
    public string Type { get; set; } = "";
    public List<string> Names { get; set; } = [];
    public List<string>? Links { get; set; } = [];
    public string? Description { get; set; }

    public override string ToString()
    {
        return string.Join(", ", Ids) + " " + (Type ?? "").TrimEnd();
    }
}
