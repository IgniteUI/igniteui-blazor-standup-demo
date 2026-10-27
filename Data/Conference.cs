namespace StandupDemo.Data;

public sealed record Session(DateTime Day, string Time, string Title, string Speaker, string Initials, string Track, int Seats, int Booked);

public sealed class Contributor
{
    public string Login { get; set; } = "";
    public string Area { get; set; } = "";
    public int Commits { get; set; }
    public int Issues { get; set; }
}

public static class Conference
{
    public static readonly DateTime FirstDay = new(2026, 11, 10);

    public static readonly IReadOnlyList<Session> Sessions =
    [
        new(FirstDay, "09:00", "Keynote: .NET in 2027", "Maya Chen", "MC", "Keynote", 900, 874),
        new(FirstDay, "10:30", "Blazor rendering deep dive", "Liam Novak", "LN", "Blazor", 300, 288),
        new(FirstDay, "11:30", "Open source UI components", "Damyan Petev", "DP", "OSS", 250, 231),
        new(FirstDay, "14:00", "Aspire for everyone", "Zoe Garcia", "ZG", "Platform", 300, 190),
        new(FirstDay.AddDays(1), "09:30", "AI agents meet Blazor", "Omar Haddad", "OH", "AI", 400, 399),
        new(FirstDay.AddDays(1), "11:00", "Theming with design tokens", "Ivy Kim", "IK", "UX", 200, 120),
        new(FirstDay.AddDays(1), "13:30", "WebAssembly perf clinic", "Theo Silva", "TS", "Blazor", 250, 241),
        new(FirstDay.AddDays(2), "10:00", "Maintaining OSS at scale", "Elena Rossi", "ER", "OSS", 200, 168),
        new(FirstDay.AddDays(2), "11:30", "Accessible components", "Kai Tanaka", "KT", "UX", 200, 151),
        new(FirstDay.AddDays(2), "15:00", "Closing panel", "Everyone", "★", "Keynote", 900, 702),
    ];

    public static List<Contributor> Contributors() =>
    [
        new() { Login = "octo-dev", Area = "Grid Lite", Commits = 412, Issues = 38 },
        new() { Login = "blazor-fan", Area = "Combo", Commits = 188, Issues = 21 },
        new() { Login = "a11y-ally", Area = "Tree", Commits = 154, Issues = 44 },
        new() { Login = "theme-wiz", Area = "Theming", Commits = 131, Issues = 12 },
        new() { Login = "wasm-ninja", Area = "Interop", Commits = 97, Issues = 9 },
        new() { Login = "doc-hero", Area = "Docs", Commits = 76, Issues = 30 },
        new() { Login = "first-pr", Area = "Chip", Commits = 3, Issues = 1 },
    ];
}
