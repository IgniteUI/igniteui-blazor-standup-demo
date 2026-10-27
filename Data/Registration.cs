using System.ComponentModel.DataAnnotations;

namespace StandupDemo.Data;

public enum TicketTier
{
    Community,
    Pro,
    Speaker
}

public sealed record Topic(string Id, string Name, string Area, string Emoji);

public sealed class Registration
{
    [Required]
    public string? Name { get; set; } = "";

    [Required, EmailAddress]
    public string? Email { get; set; } = "";
    public string? Phone { get; set; } = "";
    public string? Track { get; set; } = "blazor";
    public string[] Topics { get; set; } = ["wasm", "ai"];
    public DateTime? Day { get; set; } = new DateTime(2026, 11, 10);
    public TicketTier Tier { get; set; } = TicketTier.Community;
    public int TeamSize { get; set; } = 5;
    public double Experience { get; set; } = 4;
    public string? BadgeColor { get; set; } = "#0099FF";
    public bool Newsletter { get; set; } = true;
    [Range(typeof(bool), "true", "true", ErrorMessage = "Please agree to the code of conduct.")]
    public bool Agree { get; set; }

    public Registration Clone() => (Registration)MemberwiseClone();

    public static readonly Topic[] AllTopics =
    [
        new("wasm", "WebAssembly", "Blazor", "⚡"),
        new("webapp", "Blazor Web App", "Blazor", "🖥️"),
        new("hybrid", "MAUI Hybrid", "Blazor", "📱"),
        new("aspire", ".NET Aspire", "Platform", "🚀"),
        new("minapi", "Minimal APIs", "Platform", "🧩"),
        new("ai", "AI & agents", "Platform", "🤖"),
        new("a11y", "Accessibility", "UX", "♿"),
        new("theming", "Theming", "UX", "🎨"),
        new("perf", "Performance", "UX", "📈"),
    ];

    public static readonly (string Value, string Label)[] Tracks =
    [
        ("blazor", "Blazor deep dive"),
        ("aspnet", "ASP.NET Core"),
        ("oss", "Open source maintainers"),
        ("ux", "Design systems & UX"),
    ];
}
