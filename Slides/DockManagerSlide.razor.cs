using System.Globalization;
using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

/// <summary>
/// A trading workspace in a Dock Manager. Pane structure is a C# object graph (<see cref="BuildLayout"/>); pane bodies
/// are the slotted elements whose <c>slot</c> matches each content pane's ContentId. The watchlist selection drives the
/// chart and the order ticket; the ticket and the activity list are Lite components.
/// </summary>
public partial class DockManagerSlide
{
    private sealed record Order(string Side, int Quantity, string Symbol, string Type, double Total, DateTime Time);

    private const string AxisText = "#A9A8CF";
    private const string AxisLine = "rgba(255, 255, 255, 0.18)";
    private const string GridLine = "rgba(255, 255, 255, 0.07)";
    private const string SliceColors = "#0099FF #F05A2B #7EE2B8 #C49BFF #FFD166 #4FC3F7 #FF8A65";
    private const string SliceOutlines = "#27265F #27265F #27265F #27265F #27265F #27265F #27265F";

    private static readonly string[] candleProperties =
        [nameof(Candle.Date), nameof(Candle.Open), nameof(Candle.High), nameof(Candle.Low), nameof(Candle.Close), nameof(Candle.Volume)];

    private static readonly double PortfolioValue = Market.Allocation.Sum(s => s.Value);

    private IgbDockManager dockManager = default!;
    private IgbToast toast = default!;
    private IgbDockManagerLayout layout = BuildLayout();
    private IgbDockManagerLayout? savedLayout;
    private string toastMessage = "";

    private string symbol = Market.Watchlist[0].Symbol;
    private object[] selectedRows = [Market.Watchlist[0].Symbol];
    private string side = "Buy";
    private string quantity = "10";
    private string orderType = "Market";
    private string limitPrice = "";
    private readonly List<Order> orders = [];

    private Ticker SelectedTicker => Market.Watchlist.First(t => t.Symbol == symbol);

    private int Quantity => int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var q) ? Math.Max(q, 0) : 0;

    private double UnitPrice =>
        orderType == "Limit" && double.TryParse(limitPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out var limit) && limit > 0
            ? limit
            : SelectedTicker.Price;

    private double Estimate => Quantity * UnitPrice;

    // The layout objects are plain settings, built in code as the Dock Manager docs show.
#pragma warning disable BL0005
    private static IgbDockManagerLayout BuildLayout()
    {
        static IgbContentPane Content(string id, string header, double size = 0)
        {
            var pane = new IgbContentPane { PaneType = DockManagerPaneType.ContentPane, ContentId = id, Header = header };
            if (size > 0)
            {
                pane.Size = size;
            }

            return pane;
        }

        var root = new IgbSplitPane { PaneType = DockManagerPaneType.SplitPane, Orientation = SplitPaneOrientation.Horizontal };
        root.Panes.Add(Content("watchlist", "Watchlist", 300));

        // The center is a document host: its tabs (chart, allocation) can only be docked among documents.
        var documents = new IgbTabGroupPane { PaneType = DockManagerPaneType.TabGroupPane };
        documents.Panes.Add(Content("chart", "Price chart"));
        documents.Panes.Add(Content("allocation", "Allocation"));
        var documentRoot = new IgbSplitPane { PaneType = DockManagerPaneType.SplitPane, Orientation = SplitPaneOrientation.Horizontal };
        documentRoot.Panes.Add(documents);
        root.Panes.Add(new IgbDocumentHost { PaneType = DockManagerPaneType.DocumentHost, RootPane = documentRoot, Size = 600 });

        var side = new IgbSplitPane { PaneType = DockManagerPaneType.SplitPane, Orientation = SplitPaneOrientation.Vertical, Size = 270 };
        side.Panes.Add(Content("ticket", "Order ticket", 330));
        side.Panes.Add(Content("activity", "Activity", 200));
        root.Panes.Add(side);

        return new IgbDockManagerLayout { RootPane = root };
    }
#pragma warning restore BL0005

    // Panes moved, floated, pinned or closed in the UI: keep the field in step with what the user sees. Restore and
    // Reset then always hand the Dock Manager a different layout object, so even a second Restore applies.
    private void OnLayoutChanged(IgbDockManagerLayout current) => layout = current;

    private async Task SaveLayoutAsync()
    {
        savedLayout = await dockManager.GetCurrentLayoutAsync();
        await ShowToastAsync("Layout saved. Now drag some panes around and hit Restore.");
    }

    private async Task RestoreLayoutAsync()
    {
        if (savedLayout is null)
        {
            return;
        }

        layout = savedLayout;
        await ShowToastAsync("Layout restored");
    }

    private void ResetLayout() => layout = BuildLayout();

    // NewSelection holds the selected row items (the Ticker instances bound as Data); a primary key is handled too.
    private void OnTickerSelected(IgbRowSelectionEventArgs e)
    {
        var picked = e.Detail?.NewSelection is [var row, ..]
            ? row switch
            {
                Ticker ticker => ticker.Symbol,
                string key => key,
                _ => null,
            }
            : null;

        if (picked is not null && Market.History.ContainsKey(picked))
        {
            symbol = picked;
            selectedRows = [picked];
            limitPrice = "";
        }
    }

    private void OnSide(IgbComponentValueChangedEventArgs e) => side = e.Detail == "Sell" ? "Sell" : "Buy";

    private async Task PlaceOrderAsync()
    {
        if (Quantity <= 0)
        {
            return;
        }

        orders.Insert(0, new Order(side, Quantity, symbol, orderType, Estimate, DateTime.Now));
        await ShowToastAsync($"{side} order for {Quantity:N0} {symbol} placed");
    }

    private async Task ShowToastAsync(string message)
    {
        toastMessage = message;
        StateHasChanged();
        await toast.ShowAsync();
    }
}
