using System.Text.Json;
using PixelFill.Models.Entities;

namespace PixelFill.Models.Services;

public class ProjectFileService
{
    private readonly string _appDataPath  = FileSystem.Current.AppDataDirectory;

    public string[] GetSavedProjectNames()
    {
        var files = Directory.GetFiles(_appDataPath, "*.json");
        return files
            .Select(Path.GetFileNameWithoutExtension)
            .Where(fileName => fileName is not null)
            .Select(fileName => fileName!)
            .ToArray();
    }

    public void SaveProject(SavedCanvas canvas, string fileName)
    {
        var newFilePath = Path.Combine(_appDataPath, fileName + ".json");
        var jsonString = JsonSerializer.Serialize(canvas);
        File.WriteAllText(newFilePath, jsonString);
    }

    public SavedCanvas? LoadProject(string fileName)
    {
        var filePath = Path.Combine(_appDataPath, fileName + ".json");

        if (!File.Exists(filePath)) return null;
        
        var jsonString = File.ReadAllText(filePath);
        
        return JsonSerializer.Deserialize<SavedCanvas>(jsonString);
    }

    public bool DeleteProject(string fileName)
    {
        try
        {
            var fileToDeletePath = Path.Combine(_appDataPath, fileName + ".json");
            if (!File.Exists(fileToDeletePath))
            {
                return true;
            }

            File.Delete(fileToDeletePath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}