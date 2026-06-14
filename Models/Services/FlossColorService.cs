using HW02.Models.Entities;

namespace HW02.Models.Services;

public class FlossColorService
{
    public List<FlossColor> LoadColors()
    {
        var colors = new List<FlossColor>();

        using var stream = FileSystem.OpenAppPackageFileAsync("threadcolors_dmc_rgb.csv").GetAwaiter().GetResult();

        using var reader = new StreamReader(stream);

        reader.ReadLine();

        while ((reader.ReadLine()) is { } line)
        {
            line = line.Trim('\n', '\r');
            var values = line.Split(',');
            if (values.Length < 6) continue;

            colors.Add(new FlossColor
            {
                Floss = values[0].Trim(),
                Name = values[1].Trim(),
                Hex = $"#{values[5].Trim()}"
            });
        }

        return colors
            .OrderBy(c => int.TryParse(c.Floss, out var num) ? num : 9999)
            .ThenBy(c => c.Floss)
            .ToList();
    }
}
