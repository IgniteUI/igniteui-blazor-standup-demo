using StandupDemo.Data;

namespace StandupDemo.Charts;

public sealed class TrackStats
{
    public string Track { get; init; } = "";
    public decimal Seats { get; init; }
    public decimal Booked { get; set; }

    public static List<TrackStats> All => Conference.Sessions
        .GroupBy(s => s.Track)
        .Select(g => new TrackStats { Track = g.Key, Seats = g.Sum(s => s.Seats), Booked = g.Sum(s => s.Booked) })
        .OrderByDescending(t => t.Seats)
        .ToList();
}
