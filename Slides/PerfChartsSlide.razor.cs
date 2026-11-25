using IgniteUI.Blazor.Controls;

namespace StandupDemo.Slides;

/// <summary>One point of the live feed: three metrics on a 0–100 scale.</summary>
public sealed class FeedPoint
{
    public string Time { get; set; } = "";
    public double Cpu { get; set; }
    public double Memory { get; set; }
    public double Network { get; set; }
}

/// <summary>
/// Top: up to a million points that the browser generates (wwwroot/js/chart-data.js) and the chart pulls in by script
/// name. Bottom: a feed driven from C#, where each tick sends only the change through NotifyInsertItemAsync /
/// NotifyRemoveItemAsync instead of re-sending the data.
/// </summary>
public partial class PerfChartsSlide
{
    private sealed record Volume(string Script, string Label, int Count);

    // Chart colors are parameters, not CSS tokens: resolved values that match the deck's dark surface.
    private const string AxisText = "#A9A8CF";
    private const string AxisLine = "rgba(255, 255, 255, 0.18)";
    private const string GridLine = "rgba(255, 255, 255, 0.07)";

    private const int Window = 300;

    private static readonly Volume[] volumes =
    [
        new("Points10K", "10K", 10_000),
        new("Points100K", "100K", 100_000),
        new("Points1M", "1M", 1_000_000),
    ];

    private static readonly string[] highVolumeProperties = ["Label", "Value"];
    private static readonly string[] liveProperties = [nameof(FeedPoint.Time), nameof(FeedPoint.Cpu), nameof(FeedPoint.Memory), nameof(FeedPoint.Network)];

    private readonly List<FeedPoint> feed = [];
    private readonly Random rnd = new(42);
    private readonly CancellationTokenSource cts = new();
    private IgbCategoryChart? liveChart;
    private Volume selected = volumes[^1];
    private TimeSpan interval = TimeSpan.FromMilliseconds(16);
    private bool running = true;
    private int tick;
    private long sequence;
    private int updatesPerSecond;
    private (double Cpu, double Memory, double Network) level = (45, 60, 30);

    protected override void OnInitialized()
    {
        for (var i = 0; i < Window; i++)
        {
            feed.Add(NextPoint());
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _ = RunFeedAsync(cts.Token);
            _ = CountRateAsync(cts.Token);
        }
    }

    // Markers on every point are the most expensive part of a dense line; drop them as each series is created.
    private static void HideMarkers(IgbChartSeriesEventArgs e)
    {
        if (e.Parent is IgbCategoryChart chart)
        {
            chart.MarkerTypes.Clear();
            chart.MarkerTypes.Add(MarkerType.None);
        }
    }

    private void OnVolume(IgbComponentValueChangedEventArgs e) =>
        selected = volumes.FirstOrDefault(v => v.Script == e.Detail) ?? selected;

    private void OnRate(IgbComponentValueChangedEventArgs e)
    {
        if (int.TryParse(e.Detail, out var ms))
        {
            interval = TimeSpan.FromMilliseconds(ms);
        }
    }

    private void ToggleFeed() => running = !running;

    private async Task RunFeedAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(interval, token);
                if (!running || liveChart is null)
                {
                    continue;
                }

                var removed = feed[0];
                feed.RemoveAt(0);
                await liveChart.NotifyRemoveItemAsync(feed, 0, removed);

                var added = NextPoint();
                feed.Add(added);
                await liveChart.NotifyInsertItemAsync(feed, feed.Count - 1, added);
                tick++;
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    // The header counter re-renders once a second; the feed itself never re-renders the slide.
    private async Task CountRateAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                updatesPerSecond = tick;
                tick = 0;
                StateHasChanged();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private FeedPoint NextPoint()
    {
        static double Step(double value, double target, double noise, Random rnd) =>
            Math.Clamp(value + (target - value) * 0.04 + (rnd.NextDouble() - 0.5) * noise, 2, 98);

        sequence++;
        level = (
            Step(level.Cpu, 50 + 25 * Math.Sin(sequence / 40.0), 9, rnd),
            Step(level.Memory, 62, 3, rnd),
            Step(level.Network, rnd.NextDouble() < 0.03 ? 90 : 28, 7, rnd));

        return new FeedPoint
        {
            Time = DateTime.Now.ToString("HH:mm:ss.fff"),
            Cpu = Math.Round(level.Cpu, 1),
            Memory = Math.Round(level.Memory, 1),
            Network = Math.Round(level.Network, 1),
        };
    }

    public async ValueTask DisposeAsync()
    {
        await cts.CancelAsync();
        cts.Dispose();
    }
}
