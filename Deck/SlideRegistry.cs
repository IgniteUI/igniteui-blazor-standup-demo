using StandupDemo.Charts;
using StandupDemo.Slides;

namespace StandupDemo.Deck;

/// <summary>
/// A single slide in the deck: its URL slug, nav title, component and speaker notes.
/// <paramref name="Commercial"/> marks slides that show the commercial IgniteUI.Blazor package rather than the MIT ones.
/// </summary>
public sealed record SlideInfo(string Slug, string Title, string Icon, Type Component, string Notes, bool Commercial = false);

/// <summary>
/// The deck's running order. Reorder, add or drop slides here.
/// </summary>
public sealed class SlideRegistry
{
    public IReadOnlyList<SlideInfo> Slides { get; }

    public SlideRegistry()
    {
        Slides =
        [
            new("title", "Ignite UI for Blazor", "bolt", typeof(TitleSlide),
                "Hi! Quick intro. Ignite UI for Blazor's core components are now open source under MIT: IgniteUI.Blazor.Lite and IgniteUI.Blazor.GridLite, both 1.0 on NuGet. Everything you'll see is a running Blazor WASM app, and the deck is built from those components."),
            new("open-source", "Open source, MIT", "favorite", typeof(OpenSourceSlide),
                "Two packages, MIT licensed, built on our igniteui-webcomponents. Works in Blazor Web Apps, WebAssembly and MAUI Hybrid, following the supported .NET releases. In the explorer, search (Enter) and the chips filter the list, and Highlight marks the matches."),
            new("forms", "Forms: badge builder", "person", typeof(FormsSlide),
                "Scenario: register for a conference. Every control drives the live badge on the right: Input, Mask Input, Combo, Select, Date Picker, Radio Group, Slider, Rating, Color Picker, Switch. The QR code re-encodes as fields are filled in. It's a real form: the components take part in native validation (required name and code of conduct, email format), and a valid submit opens a confirmation Dialog, then a Snackbar with Undo."),
            new("getting-started", "Getting started", "code", typeof(GettingStartedSlide),
                "Getting started in four steps, and a layout demo at the same time: Splitter, Tree, Breadcrumbs, Stepper, Tabs. Clicking a file in the Tree moves the Stepper, and the Stepper selects the Tree item back. Tabs show Blazor Web App, WebAssembly and .NET MAUI Blazor Hybrid registration. Available now: the dotnet-blazor plugin in dotnet/skills has a use-igniteui-blazor skill, so Copilot CLI or Claude Code can do the setup. Coming soon: `dotnet scaffold aspnet blazor-igniteui` does steps 1-3 in one go (dotnet/Scaffolding#3823)."),
            new("grid-lite", "Grid Lite", "grid", typeof(GridLiteSlide),
                $"Grid Lite: MIT, virtualized, sort, filter, resize. {GridLiteSlide.RowCount:N0} rows here. The search box, Range Slider and chips are bound to the same filter state as the grid's FilterExpressions, so filtering from either side updates both; the chips also toggle columns. Needs editing or paging later? Upgrade to the commercial IgbGrid."),
            new("charts", "ApexCharts", "trending_up", typeof(ChartsSlide),
                "Blazor-ApexCharts is a great pairing with Ignite UI Lite. Button Group switches series type, chips pick tracks, the Slider boosts demand, and Grid Lite shows the same data."),
            new("dashboard", "Dashboard", "dashboard", typeof(DashboardSlide),
                "Tile Manager: drag tiles by the header, resize, maximize, fullscreen. Save the layout, mess it up, restore it: SaveLayoutAsync / LoadLayoutAsync. Tiles hold Calendar, List, progress indicators, Carousel and a Grid Lite."),
            new("theming", "Theming", "palette", typeof(ThemingSlide),
                "Theming is plain CSS custom properties. Theme Provider switches Material / Bootstrap / Fluent / Indigo for one region at runtime. Palette presets: the Ignite UI brand palette, each design system's stock palette, or Custom colors. Roundness, spacing and size start at each theme's defaults; flip the switch to override them. The palettes came from the igniteui-theming MCP."),
            new("ai", "AI-ready", "chat", typeof(AiSlide),
                "Chat (preview) as an assistant UI: suggestions, typing indicator, client-side templates. On the right: install the Agent Skills with `gh skill install IgniteUI/igniteui-blazor`, or use `ig ai-config`, which also adds agent rules (AGENTS.md etc.) and the MCP servers. Coming soon: Figma to code. This deck was built with them."),
            new("upgrade", "Going further", "upgrade", typeof(UpgradeSlide),
                "When an app outgrows Lite: the commercial IgniteUI.Blazor. New in 26.2 (beta on NuGet as IgniteUI.Blazor.Trial): it builds on IgniteUI.Blazor.Lite, it's a package dependency. So nothing gets replaced: same namespace, same AddIgniteUIBlazor(), same theme, and every Lite component and Grid Lite keep working. You add one PackageReference and the grid stylesheet. The next slides run on the Trial package, right next to the MIT components in this deck.",
                Commercial: true),
            new("grid", "Grid", "table", typeof(GridSlide),
                $"The upgrade from Grid Lite: IgbGrid, same {GridSlide.RowCount:N0} employees. Grouped by department with the group area, salary and rating summaries per group, multi-row selection, row editing from the action strip, row pinning, column pinning and hiding from the toolbar, Excel-style filtering, export to Excel/CSV. The Rating column is a Lite IgbRating in a Blazor cell template. The side panel uses the grid API: GroupByAsync, SelectRowsAsync; the counters come from its RowSelectionChanging and RowEditDone events.",
                Commercial: true),
            new("grid-family", "Tree, hierarchical, pivot", "account_tree", typeof(GridFamilySlide),
                "The rest of the grid family, same column API. Tree Grid: an org chart from flat data with ManagerId as the ForeignKey, summaries per level. Hierarchical Grid: country, then department, then people, each level its own schema through row islands. Pivot Grid: salary by department and country; drag dimensions in the data selector on the left to reshape it.",
                Commercial: true),
            new("perf-charts", "High-performance charts", "speed", typeof(PerfChartsSlide),
                "65+ chart types. Top: one million points, generated in the browser and handed over through DataSourceScript, so they never cross the .NET/JS boundary. Zoom with the mouse wheel or drag, it stays smooth. Bottom: a live feed driven from C#: a PeriodicTimer appends a point and calls NotifyInsertItemAsync / NotifyRemoveItemAsync, so only the change goes to the chart.",
                Commercial: true),
            new("dock-manager", "Dock Manager", "view_quilt", typeof(DockManagerSlide),
                "Dock Manager: an IDE-style workspace. Drag a pane header to dock it anywhere, drop it outside to float it, pin and unpin, maximize, tabs in the document host. Picking a ticker in the watchlist grid drives the financial chart. The order ticket is all Lite components. Save the layout with GetCurrentLayoutAsync, mess it up, restore it.",
                Commercial: true),
            new("thanks", "Thank you", "star", typeof(ThanksSlide),
                "Thanks! Star the repos, file issues, send PRs. The QR codes go to both repos."),
        ];
    }

    public int IndexOf(string? slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return 0;
        }

        for (var i = 0; i < Slides.Count; i++)
        {
            if (string.Equals(Slides[i].Slug, slug, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }
}
