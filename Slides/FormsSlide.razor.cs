using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

public partial class FormsSlide
{
    private Registration model = new() { Name = "Ada Lovelace", Email = "ada@example.com" };
    private Registration? beforeSubmit;
    private IgbDialog dialog = default!;
    private IgbSnackbar snackbar = default!;

    private readonly DateTime? minDay = new DateTime(2026, 11, 10);
    private readonly DateTime? maxDay = new DateTime(2026, 11, 12);
    private static readonly string[] badgeSwatches = ["#0099FF", "#F05A2B", "#7EE2B8", "#C49BFF", "#FFD166", "#27265F"];

    private int Completeness
    {
        get
        {
            bool[] filled =
            [
                !string.IsNullOrWhiteSpace(model.Name),
                !string.IsNullOrWhiteSpace(model.Email),
                model.Phone?.Count(char.IsDigit) >= 9,
                model.Topics.Length > 0,
                model.Day is not null,
                model.Agree,
            ];
            return filled.Count(f => f) * 100 / filled.Length;
        }
    }

    private string Initials
    {
        get
        {
            var parts = (model.Name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length switch
            {
                0 => "?",
                1 => parts[0][..1].ToUpperInvariant(),
                _ => $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
            };
        }
    }

    private string TrackLabel => Registration.Tracks.FirstOrDefault(t => t.Value == model.Track).Label ?? "Pick a track";

    private List<Topic> SelectedTopics => Registration.AllTopics.Where(t => model.Topics.Contains(t.Id)).ToList();

    private StyleVariant TierVariant => model.Tier switch
    {
        TicketTier.Speaker => StyleVariant.Warning,
        TicketTier.Pro => StyleVariant.Success,
        _ => StyleVariant.Info
    };

    private string BadgeStyle => $"--badge-color: {(string.IsNullOrEmpty(model.BadgeColor) ? "#0099FF" : model.BadgeColor)}";

    private string VCard =>
        $"BEGIN:VCARD\nVERSION:3.0\nFN:{model.Name}\nEMAIL:{model.Email}\nTEL:{model.Phone}\nNOTE:{model.Tier} · {TrackLabel}\nEND:VCARD";

    private void RemoveTopic(string id) => model.Topics = model.Topics.Where(t => t != id).ToArray();

    private void Reset() => model = new Registration();

    // The browser has validated the form (required name and code of conduct, email format) before this runs.
    private Task Submit() => dialog.ShowAsync();

    private async Task Confirm()
    {
        await dialog.HideAsync();
        beforeSubmit = model.Clone();
        model = new Registration();
        await snackbar.ShowAsync();
    }

    private void Undo(IgbVoidEventArgs e)
    {
        if (beforeSubmit is not null)
        {
            model = beforeSubmit;
            beforeSubmit = null;
        }
    }
}
