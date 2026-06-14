using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

using HW02.Models.Services;
using HW02.ViewModels;
using HW02.Views;

namespace HW02;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddSingleton<ProjectFileService>();
        builder.Services.AddSingleton<FlossColorService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainView>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}