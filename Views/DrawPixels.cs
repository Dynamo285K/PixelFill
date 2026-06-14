using HW02.Models.Entities;
namespace HW02.Views;

public class DrawPixels : IDrawable
{
    public Pixel[,]? PixelMatrix { get; set; }
    public int GridSize { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (PixelMatrix == null || GridSize == 0) return;
        
        var minDimension = Math.Min(dirtyRect.Width, dirtyRect.Height);
        var cellSize = minDimension / GridSize;

        var gridWidth = cellSize * GridSize;
        var gridHeight = cellSize * GridSize;

        var offsetX = (dirtyRect.Width - gridWidth) / 2;
        var offsetY = (dirtyRect.Height - gridHeight) / 2;


        for (var row = 0; row < GridSize; row++)
        {
            for (var col = 0; col < GridSize; col++)
            {
                var pixel = PixelMatrix[row, col];
                
                var xPos = offsetX + (col * cellSize);
                var yPos = offsetY + (row * cellSize);
                
                canvas.FillColor = Color.FromArgb(pixel.HexColor);
                canvas.FillRectangle(xPos, yPos, cellSize, cellSize);
                
                canvas.StrokeColor = Colors.LightGray;
                canvas.StrokeSize = 0.5f;
                canvas.DrawRectangle(xPos, yPos, cellSize, cellSize);
            }
        }
    }
}