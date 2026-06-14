using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PixelFill.Models.Entities;
using PixelFill.Models.Services;

namespace PixelFill.ViewModels;

public partial class MainViewModel(IDialogService dialogService, ProjectFileService fileService, FlossColorService flossColorService) : ObservableObject
{

    public List<FlossColor> Colors { get; } = flossColorService.LoadColors();
    private readonly Stack<UndoAction> _undoHistory = new();

    [ObservableProperty] 
    public partial Pixel[,]? PixelMatrix { get; set; }

    [ObservableProperty]
    public partial int GridSize { get; set; }

    [ObservableProperty]
    public partial string GridSizeInput { get; set; } = "20";

    [ObservableProperty]
    public partial bool IsEraserActive { get; set; }

    [ObservableProperty]
    public partial FlossColor? SelectedColor { get; set; }

    [RelayCommand]
    private async Task CreateGrid()
    {
        if (!int.TryParse(GridSizeInput, out var size) || size < 5 || size > 100)
        {
            await dialogService.ShowAlertAsync("Error", "Enter number from 5 to 100.", "OK");
            return;
        }

        var newMatrix = new Pixel[size, size];
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
                newMatrix[row, col] = new Pixel { X = col, Y = row, HexColor = "#FFFFFF" };
        }

        GridSize = size;
        GridSizeInput = size.ToString();
        _undoHistory.Clear();
        PixelMatrix = newMatrix;
    }

    public void PaintAtPoint(double x, double y, double canvasWidth, double canvasHeight, string targetHexColor)
    {
        if (PixelMatrix == null || GridSize == 0) return;
        
        var minSize = Math.Min(canvasWidth, canvasHeight);
        var cellSize = minSize / GridSize;
        var gridWidth = cellSize * GridSize;
        var gridHeight = cellSize * GridSize;

        var offsetX = (canvasWidth - gridWidth) / 2;
        var offsetY = (canvasHeight - gridHeight) / 2;

        var adjustedX = x - offsetX;
        var adjustedY = y - offsetY;

        if (adjustedX < 0 || adjustedX >= gridWidth || adjustedY < 0 || adjustedY >= gridHeight) return;

        var col = (int)(adjustedX / cellSize);
        var row = (int)(adjustedY / cellSize);
    
        if (col < 0 || col >= GridSize || row < 0 || row >= GridSize) return;
        
        var oldColor = PixelMatrix[row, col].HexColor;
        if (oldColor == targetHexColor) return;
        
        _undoHistory.Push(new UndoAction { Row = row, Col = col, PreviousColor = oldColor });
        PixelMatrix[row, col].HexColor = targetHexColor;
        
        OnPropertyChanged(nameof(PixelMatrix));
    }

    [RelayCommand]
    private void ClearCanvas()
    {
        if (PixelMatrix == null) return;
        for (var row = 0; row < GridSize; row++)
            for (var col = 0; col < GridSize; col++)
                PixelMatrix[row, col].HexColor = "#FFFFFF";
        
        _undoHistory.Clear();
        OnPropertyChanged(nameof(PixelMatrix));
    }

    [RelayCommand]
    private void UndoLastAction()
    {
        if (PixelMatrix == null || _undoHistory.Count == 0) return;
        var lastAction = _undoHistory.Pop();
        PixelMatrix[lastAction.Row, lastAction.Col].HexColor = lastAction.PreviousColor;
        OnPropertyChanged(nameof(PixelMatrix));
    }

    [RelayCommand]
    private void ToggleEraser()
    {
        IsEraserActive = !IsEraserActive;

        if (IsEraserActive)
            SelectedColor = null;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (PixelMatrix == null) return;
        
        var fileName = await dialogService.ShowPromptAsync("Save Canvas", "Name (No spaces, no special signs):", "OK", "Cancel");
        if (string.IsNullOrEmpty(fileName)) return;
        
        var existingFiles = fileService.GetSavedProjectNames();
        if (!existingFiles.Contains(fileName) && existingFiles.Length >= 5)
        {
            await dialogService.ShowAlertAsync("Limit reached", "You can only save up to 5 projects", "OK");
            return;
        }
        
        var savedCanvas = new SavedCanvas { GridSize = GridSize, Pixels = [] };
        foreach (var pixel in PixelMatrix) savedCanvas.Pixels.Add(pixel);
        
        fileService.SaveProject(savedCanvas, fileName);
        await dialogService.ShowToastAsync("Canvas was successfully saved");
    }

    [RelayCommand]
    private async Task Load()
    {
        var fileNames = fileService.GetSavedProjectNames();
        if (fileNames.Length == 0)
        {
            await dialogService.ShowAlertAsync("Error", "No files found", "OK");
            return;
        }

        var selectedProject = await dialogService.ShowActionSheetAsync("Choose file to load:", "Cancel", null, fileNames);
        if (selectedProject == "Cancel" || string.IsNullOrEmpty(selectedProject)) return;

        var loadedCanvas = fileService.LoadProject(selectedProject);
        if (loadedCanvas == null)
        {
            await dialogService.ShowAlertAsync("Error", "Unable to load selected project.", "OK");
            return;
        }
            
        GridSize = loadedCanvas.GridSize;
        var newMatrix = new Pixel[GridSize, GridSize];
        foreach (var pixel in loadedCanvas.Pixels) newMatrix[pixel.Y, pixel.X] = pixel;
            
        GridSizeInput = GridSize.ToString();
        _undoHistory.Clear();
        PixelMatrix = newMatrix;
        
        await dialogService.ShowToastAsync($"Project '{selectedProject}' was successfully loaded.");
    }

    [RelayCommand]
    private async Task DeleteProject()
    {
        var fileNames = fileService.GetSavedProjectNames();
        if (fileNames.Length == 0)
        {
            await dialogService.ShowAlertAsync("Info", "No files to delete.", "OK");
            return;
        }
        
        var selectedProject = await dialogService.ShowActionSheetAsync("Choose file to delete:", "Cancel", null, fileNames);
        if (selectedProject == "Cancel" || string.IsNullOrEmpty(selectedProject)) return;

        var confirmed = await dialogService.ShowConfirmationAsync("Confirm Delete", $"Are you sure you want to permanently delete '{selectedProject}'?", "Yes", "No");
        if (!confirmed) return;
        
        if (fileService.DeleteProject(selectedProject))
            await dialogService.ShowToastAsync($"Project '{selectedProject}' was successfully deleted.");
        else
            await dialogService.ShowAlertAsync("Error", "Unable to delete selected project.", "OK");
    }
}