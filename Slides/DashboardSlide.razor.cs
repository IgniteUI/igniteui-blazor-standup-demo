using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

public partial class DashboardSlide
{
    private static readonly (string Title, string Text, string Accent)[] highlights =
    [
        ("MIT licensed", "Lite and Grid Lite are free for any project, commercial included.", "#0099FF"),
        ("Web components inside", "The same igniteui-webcomponents engine as our Angular and React products.", "#F05A2B"),
        ("AI-ready", "Agent Skills plus theming and docs MCP servers.", "#7EE2B8"),
        ("Contributions welcome", "Issues, ideas and pull requests are welcome on GitHub.", "#C49BFF"),
    ];

    private readonly List<Contributor> contributors = Conference.Contributors();
    private IgbTileManager tiles = default!;
    private IgbToast savedToast = default!;
    private IgbToast restoredToast = default!;
    private TileManagerDragMode dragMode = TileManagerDragMode.TileHeader;
    private DateTime selectedDay = Conference.FirstDay;
    private string? savedLayout;

    private readonly int capacity = Conference.Sessions.Sum(s => s.Seats);
    private readonly int registered = Conference.Sessions.Sum(s => s.Booked);

    // The calendar reports the picked day as a UTC instant; it is converted to local time when bound.
    private List<Session> DaySessions => Conference.Sessions.Where(s => s.Day.Date == selectedDay.Date).ToList();

    private static StyleVariant TrackVariant(string track) => track switch
    {
        "Blazor" => StyleVariant.Primary,
        "OSS" => StyleVariant.Success,
        "AI" => StyleVariant.Warning,
        "UX" => StyleVariant.Info,
        _ => StyleVariant.Danger
    };

    private void OnDragMode(IgbComponentValueChangedEventArgs e) => dragMode = e.Detail switch
    {
        "tile" => TileManagerDragMode.Tile,
        "none" => TileManagerDragMode.None,
        _ => TileManagerDragMode.TileHeader
    };

    private async Task SaveLayout()
    {
        savedLayout = await tiles.SaveLayoutAsync();
        await savedToast.ShowAsync();
    }

    private async Task RestoreLayout()
    {
        if (savedLayout is not null)
        {
            await tiles.LoadLayoutAsync(savedLayout);
            await restoredToast.ShowAsync();
        }
    }
}
