namespace PixelFill.Models.Entities;

public class UndoAction
{
    public int Row { get; init; }
    public int Col { get; init; }
    public required string PreviousColor { get; init; }
}