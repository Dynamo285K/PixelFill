using HW02.ViewModels;

namespace HW02.Views;

public partial class MainView
{
    private readonly DrawPixels _drawable;
    private double _baseCanvasSize;
    private readonly MainViewModel _viewModel;
    private Models.Entities.Pixel[,]? _currentMatrixReference;

    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _drawable = new DrawPixels();
        StitchCanvas.Drawable = _drawable;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainViewModel.PixelMatrix) &&
            e.PropertyName != nameof(MainViewModel.GridSize)) return;
        
        _drawable.PixelMatrix = _viewModel.PixelMatrix;
        _drawable.GridSize = _viewModel.GridSize;

        if (_currentMatrixReference != _viewModel.PixelMatrix)
        {
            _currentMatrixReference = _viewModel.PixelMatrix;

            if (_viewModel.GridSize > 0)
            {
                _baseCanvasSize = Math.Min(CanvasScrollView.Width, CanvasScrollView.Height) - 40;
                if (_baseCanvasSize <= 0) _baseCanvasSize = 500;

                StitchCanvas.WidthRequest = _baseCanvasSize;
                StitchCanvas.HeightRequest = _baseCanvasSize;
                ZoomSlider.Value = 1;
            }
        }
        StitchCanvas.Invalidate();
    }
    
    private void OnCanvasClicked(object? sender, TouchEventArgs e)
    {
        var targetHexColor = "#FFFFFF";

        if (!_viewModel.IsEraserActive)
        {
            var selectedColor = _viewModel.SelectedColor;
            if (selectedColor != null)
                targetHexColor = selectedColor.Hex;
            else
            {
                DisplayAlertAsync("Info", "First choose a color from pallet!", "OK");
                return;
            }
        }
        
        _viewModel.PaintAtPoint(e.Touches[0].X, e.Touches[0].Y, StitchCanvas.Width, StitchCanvas.Height, targetHexColor);
    }
    
    private void OnZoomSliderChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_viewModel.PixelMatrix == null || _baseCanvasSize == 0) return;
        
        var newSize = (int)Math.Round(_baseCanvasSize * e.NewValue);
        if ((int)StitchCanvas.WidthRequest == newSize && (int)StitchCanvas.HeightRequest == newSize) return;
        
        StitchCanvas.HeightRequest = newSize;
        StitchCanvas.WidthRequest = newSize;
        StitchCanvas.Invalidate();
    }

    // Not working properly. Keeps lagging and zooming is not smooth.
    private void OnCanvasPinched(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status != GestureStatus.Running) return;

        var newZoom = ZoomSlider.Value * e.Scale;
        ZoomSlider.Value = Math.Clamp(newZoom, ZoomSlider.Minimum, ZoomSlider.Maximum);
    }
}