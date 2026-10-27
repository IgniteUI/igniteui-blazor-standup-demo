using IgniteUI.Blazor.Controls;

namespace StandupDemo.Slides;

public partial class GettingStartedSlide
{
    private sealed record ProjectFile(string Name, string? Folder, int Step);

    // The Tree and the Stepper both drive (and follow) the same step index.
    private static readonly ProjectFile[] files =
    [
        new("MyApp.csproj", null, 0),
        new("Program.cs", null, 1),
        new("index.html", "wwwroot", 2),
        new("Home.razor", "Pages", 3),
    ];

    private static readonly string[] themes = ["Material", "Bootstrap", "Fluent", "Indigo"];

    private IgbStepper stepper = default!;
    private int step;
    private string theme = "Material";
    private bool dark = true;
    private double stars = 3;

    private ProjectFile CurrentFile => files.First(f => f.Step == step);

    private Theme ThemeEnum => Enum.Parse<Theme>(theme);

    private async Task GoTo(int index)
    {
        if (index < 0 || index >= files.Length)
        {
            return;
        }

        step = index;
        await stepper.NavigateToAsync(index);
    }

    private void OnStepChanged(IgbActiveStepChangedEventArgs e) => step = (int)e.Detail.Index;

    private void OnThemePicked(IgbComponentValueChangedEventArgs e) => theme = e.Detail ?? theme;
}
