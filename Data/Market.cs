namespace StandupDemo.Data;

/// <summary>A watchlist row. <see cref="Change"/> is the last day's move as a fraction (0.012 = +1.2%).</summary>
public sealed class Ticker
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string Sector { get; set; } = "";
    public double Price { get; set; }
    public double Change { get; set; }
    public int Shares { get; set; }
}

/// <summary>One trading day; the financial chart needs Open/High/Low/Close, Volume and a date.</summary>
public sealed class Candle
{
    public DateTime Date { get; set; }
    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
    public double Volume { get; set; }
}

/// <summary>Holdings value per sector, for the allocation ring.</summary>
public sealed record SectorSlice(string Sector, double Value);

/// <summary>Deterministic, made-up market data: fictional tickers, a random walk per symbol.</summary>
public static class Market
{
    private const int Days = 180;

    private static readonly (string Symbol, string Name, string Sector, double Start, int Shares)[] Listings =
    [
        ("IGNT", "Ignite Labs", "Software", 142, 120),
        ("BLZR", "Blazor Works", "Software", 88, 200),
        ("RAZR", "Razor Pages Inc.", "Software", 54, 150),
        ("WASM", "Wasm Silicon", "Semiconductors", 211, 60),
        ("LINQ", "Linq Capital", "Finance", 73, 180),
        ("SGNL", "SignalR Telecom", "Telecom", 39, 400),
        ("KSTL", "Kestrel Air", "Transport", 26, 500),
        ("NUGT", "NuGet Logistics", "Transport", 61, 140),
        ("AZUR", "Azure Coast Energy", "Energy", 97, 90),
        ("MAUI", "Maui Resorts", "Leisure", 45, 220),
    ];

    public static IReadOnlyDictionary<string, List<Candle>> History { get; } = Listings
        // Seeds far apart: nearby seeds give System.Random sequences that move together.
        .Select((l, i) => (l.Symbol, Candles: Walk(l.Start, seed: 7_919 * (i + 3))))
        .ToDictionary(x => x.Symbol, x => x.Candles);

    public static List<Ticker> Watchlist { get; } = Listings
        .Select(l =>
        {
            var history = History[l.Symbol];
            var last = history[^1];
            var previous = history[^2];
            return new Ticker
            {
                Symbol = l.Symbol,
                Name = l.Name,
                Sector = l.Sector,
                Price = last.Close,
                Change = Math.Round((last.Close - previous.Close) / previous.Close, 4),
                Shares = l.Shares,
            };
        })
        .ToList();

    public static List<SectorSlice> Allocation { get; } = Watchlist
        .GroupBy(t => t.Sector)
        .Select(g => new SectorSlice(g.Key, Math.Round(g.Sum(t => t.Price * t.Shares))))
        .OrderByDescending(s => s.Value)
        .ToList();

    private static List<Candle> Walk(double start, int seed)
    {
        var rnd = new Random(seed);
        var date = new DateTime(2026, 11, 24).AddDays(-Days * 7 / 5);
        var close = start;
        var candles = new List<Candle>(Days);
        while (candles.Count < Days)
        {
            date = date.AddDays(1);
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            var open = close * (1 + (rnd.NextDouble() - 0.5) * 0.01);
            close = Math.Max(5, open * (1 + (rnd.NextDouble() - 0.48) * 0.04));
            var high = Math.Max(open, close) * (1 + rnd.NextDouble() * 0.012);
            var low = Math.Min(open, close) * (1 - rnd.NextDouble() * 0.012);
            candles.Add(new Candle
            {
                Date = date,
                Open = Math.Round(open, 2),
                High = Math.Round(high, 2),
                Low = Math.Round(low, 2),
                Close = Math.Round(close, 2),
                Volume = Math.Round(200_000 + rnd.NextDouble() * 800_000),
            });
        }

        return candles;
    }
}
