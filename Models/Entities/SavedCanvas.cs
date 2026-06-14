namespace HW02.Models.Entities;

public class SavedCanvas
{
    public int GridSize { get; init; }
    public List<Pixel> Pixels { get; init; } = [];
}