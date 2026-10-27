namespace StandupDemo.Deck;

public sealed record Presenter(string Name, string GitHub, string Avatar);

/// <summary>Who's presenting: shown on the title and thank-you slides.</summary>
public static class Presenters
{
    public static readonly IReadOnlyList<Presenter> All =
    [
        new("Damyan Petev", "damyanpetev", "img/damyan-petev.jpg"),
        new("Konstantin Dinev", "kdinev", "img/konstantin-dinev.jpg"),
    ];
}
