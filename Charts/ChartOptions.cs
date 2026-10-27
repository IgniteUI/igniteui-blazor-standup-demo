using ApexCharts;

namespace StandupDemo.Charts;

/// <summary>Shared dark styling so Blazor-ApexCharts sits nicely on the deck's navy theme.</summary>
public static class ChartOptions
{
    public static ApexChartOptions<T> Dark<T>() where T : class => new()
    {
        Chart = new Chart
        {
            Background = "transparent",
            ForeColor = "#C6C5E2",
            FontFamily = "Titillium Web, system-ui, sans-serif",
            Toolbar = new Toolbar { Show = false },
        },
        Theme = new global::ApexCharts.Theme { Mode = Mode.Dark },
        Colors = ["#0099FF", "#F05A2B", "#7EE2B8", "#C49BFF"],
        Grid = new Grid { BorderColor = "rgba(255,255,255,0.08)" },
        PlotOptions = new PlotOptions { Bar = new PlotOptionsBar { BorderRadius = 6, ColumnWidth = "55%" } },
        DataLabels = new DataLabels { Enabled = false },
        Legend = new Legend { Position = LegendPosition.Top },
    };
}
