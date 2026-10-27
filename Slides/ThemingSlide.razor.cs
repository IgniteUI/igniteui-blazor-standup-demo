using System.Globalization;
using IgniteUI.Blazor.Controls;

namespace StandupDemo.Slides;

public partial class ThemingSlide
{
    private sealed record PalettePreset(string Key, string Label, string Primary, string Secondary, string Surface);

    private static readonly CultureInfo inv = CultureInfo.InvariantCulture;
    private static readonly string[] swatches = ["#0099FF", "#F05A2B", "#7EE2B8", "#C49BFF", "#FFD166", "#FF4F8B"];

    // "brand" is the deck's own palette (ig-palette.css). The design-system presets are the stock dark
    // palettes from the theming MCP (lab-palettes.css). "custom" derives ramps from two color pickers.
    private static readonly PalettePreset[] palettes =
    [
        new("brand", "Ignite UI", "#0099FF", "#F05A2B", "#27265F"),
        new("bootstrap", "Bootstrap", "#0d6efd", "#6c757d", "rgb(33, 37, 41)"),
        new("material", "Material", "#09f", "#df1b74", "rgb(34, 34, 34)"),
        new("fluent", "Fluent", "#0078d4", "#2b88d8", "rgb(27, 27, 27)"),
        new("indigo", "Indigo", "rgb(63, 81, 181)", "rgb(63, 81, 181)", "rgb(30, 31, 36)"),
        new("custom", "Custom", "#FF4F8B", "#FFD166", "#27265F"),
    ];

    private Theme theme = Theme.Material;
    private string palette = "brand";
    private string? primary = "#FF4F8B";
    private string? secondary = "#FFD166";
    private bool overrideLayout;
    private double roundness = 0.5;
    private double spacing = 1;
    private int size = 2;
    private readonly DateTime? labDate = new DateTime(2026, 11, 10);

    private PalettePreset Current => palettes.First(p => p.Key == palette);
    private string PaletteLabel => Current.Label;

    // Only what the user changed goes inline; everything else is the theme's / palette's default.
    private string LabStyle
    {
        get
        {
            var style = palette == "custom" ? $"--seed-primary: {primary}; --seed-secondary: {secondary};" : "";
            if (overrideLayout)
            {
                style += string.Create(inv, $" --ig-radius-factor: {roundness}; --ig-spacing: {spacing}; --ig-size: {size};");
            }

            return style;
        }
    }

    private string GeneratedCss
    {
        get
        {
            var palettePart = palette switch
            {
                "brand" => "/* palette: create_palette + create_custom_palette (brand 500s) → ig-palette.css */",
                "custom" => $"/* palette: create_palette(primary: \"{primary}\", secondary: \"{secondary}\", variant: \"dark\") */",
                _ => $"/* palette: create_palette(...the dark-{palette}-palette preset) */",
            };
            var layout = overrideLayout
                ? string.Create(inv, $"\n:root {{ --ig-radius-factor: {roundness:0.0}; --ig-spacing: {spacing:0.00}; --ig-size: {size}; }}")
                : "\n/* roundness, spacing, size: theme defaults (set_roundness / set_spacing / set_size) */";
            var link = $"\n<link href=\"_content/IgniteUI.Blazor/themes/dark/{theme.ToString().ToLowerInvariant()}.css\" rel=\"stylesheet\" />";
            return palettePart + layout + link;
        }
    }

    private void OnTheme(IgbComponentValueChangedEventArgs e)
    {
        if (Enum.TryParse<Theme>(e.Detail, out var picked))
        {
            theme = picked;
        }
    }

    private void OnSize(IgbComponentValueChangedEventArgs e)
    {
        if (int.TryParse(e.Detail, inv, out var picked))
        {
            size = picked;
        }
    }

    // Chips act as a single-select group; deselecting the active one keeps it selected.
    private void PickPalette(string key, bool on)
    {
        if (on)
        {
            palette = key;
        }
    }

    private void Reset()
    {
        theme = Theme.Material;
        palette = "brand";
        overrideLayout = false;
        roundness = 0.5;
        spacing = 1;
        size = 2;
    }
}
