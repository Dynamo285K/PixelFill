using CommunityToolkit.Maui.Alerts;

namespace HW02.Models.Services;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel);
    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
    Task<string?> ShowPromptAsync(string title, string message, string accept, string cancel);
    Task<string> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons);
    Task ShowToastAsync(string message);
}

public class DialogService : IDialogService
{
    private Page CurrentPage => Application.Current!.Windows[0].Page!;

    public Task ShowAlertAsync(string title, string message, string cancel)
        => CurrentPage.DisplayAlertAsync(title, message, cancel);

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        => CurrentPage.DisplayAlertAsync(title, message, accept, cancel);

    public Task<string?> ShowPromptAsync(string title, string message, string accept, string cancel)
        => CurrentPage.DisplayPromptAsync(title, message, accept, cancel);

    public Task<string> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons)
        => CurrentPage.DisplayActionSheetAsync(title, cancel, destruction, buttons);

    public Task ShowToastAsync(string message)
        => Toast.Make(message).Show();
}