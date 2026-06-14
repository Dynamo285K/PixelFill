# PixelFill

PixelFill is a .NET MAUI application for creating pixel-style patterns with a DMC embroidery floss color palette. The user creates a square grid, selects a color from the palette, and fills individual cells. Finished canvases can be saved, loaded again, or deleted.

## What This README Contains

This file is a quick entry point for the project. It contains:

- a short description of the application,
- main features,
- used technologies,
- development environment requirements,
- setup and run instructions,
- basic application usage,
- information about data storage,
- an overview of the project structure,
- known limitations and possible future improvements.

## Features

- create a square grid with a size from 5 to 100 cells,
- draw individual pixels,
- use a color palette loaded from a CSV file with DMC colors,
- erase cells back to white,
- undo the last action using the Undo button or `Ctrl+Z` / `Cmd+Z`,
- clear the entire canvas,
- zoom in and out using a slider,
- save, load, and delete projects.

## Technologies

- .NET MAUI
- C#
- XAML
- CommunityToolkit.Maui
- CommunityToolkit.Mvvm
- Microsoft.Maui.Controls

The project uses the MVVM approach. The main UI logic is in `ViewModels/MainViewModel.cs`, canvas drawing is handled by `Views/DrawPixels.cs`, and file operations are handled by `Models/Services/ProjectFileService.cs`.

## Requirements

To develop and run the project, you need:

- .NET 10 SDK,
- MAUI workload,
- platform-specific tools depending on the target platform:
  - Android SDK for Android,
  - Xcode for iOS or Mac Catalyst,
  - Windows SDK for Windows.

The MAUI workload can be installed with:

```bash
dotnet workload install maui
```

## Installation and Running

First, restore dependencies:

```bash
dotnet restore PixelFill.sln
```

Build the project with:

```bash
dotnet build PixelFill.sln
```

Running the app depends on the target platform. Examples:

```bash
dotnet build -t:Run -f net10.0-maccatalyst PixelFill.csproj
```

```bash
dotnet build -t:Run -f net10.0-android PixelFill.csproj
```

```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0 PixelFill.csproj
```

For iOS, it is usually most practical to run the project through an IDE or with a specifically configured simulator or device.

## Using the Application

1. Enter the grid size in the `Grid Size` field.
2. Click `Create canvas`.
3. Select a color from the palette at the bottom of the screen.
4. Click cells to color the canvas.
5. Use the eraser icon button to erase cells.
6. Revert the last change with `Undo`, `Ctrl+Z`, or `Cmd+Z`.
7. Use the `Zoom` slider to change the canvas size.
8. Save a project with `Save`, load it with `Load`, and remove it with `Delete Project`.

## Project Storage

Saved projects are JSON files stored in the platform-specific application data directory. One saved project contains:

- the grid size,
- a list of pixels,
- the coordinates of each pixel,
- the pixel color in HEX format.

The application currently allows up to 5 saved projects. When saving, it is best to use simple names without spaces or special characters.

## Project Structure

```text
.
├── App.xaml
├── AppShell.xaml
├── PixelFill.csproj
├── PixelFill.sln
├── MauiProgram.cs
├── Models
│   ├── Entities
│   └── Services
├── Resources
│   ├── Raw
│   ├── Images
│   ├── Fonts
│   └── Styles
├── ViewModels
└── Views
```

Important files:

- `Views/MainView.xaml` - the main user interface,
- `Views/MainView.xaml.cs` - handling canvas interaction, zoom, and clicks,
- `Views/DrawPixels.cs` - drawing the pixel grid,
- `ViewModels/MainViewModel.cs` - application state, commands, and canvas logic,
- `Models/Services/FlossColorService.cs` - loading DMC colors from CSV,
- `Models/Services/ProjectFileService.cs` - saving, loading, and deleting projects,
- `Resources/Raw/threadcolors_dmc_rgb.csv` - the source color palette.

## Known Limitations

- Pinch zoom is marked in the code as not fully smooth.
- A maximum of 5 projects can be saved.
- Exporting the finished image or a PDF is not implemented yet.
- Saved project names should be entered without spaces or special characters.

## Possible Improvements

- export patterns to PNG or PDF,
- better project name validation when saving,
- smoother pinch zoom,
- bulk replacement of colors already used on the canvas,
- display the number of used colors and a material list.
