using System.Globalization;
using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

/// <summary>
/// One filter state per column drives the side panel (derived from it) and the match count.
/// Side panel changes go to the grid through FilterExpressions; changes made in the grid's own UI
/// come back through the Filtered event and only update the state, since the grid already has them.
/// Sorting works the same way with SortingExpressions / Sorted.
/// </summary>
public partial class GridLiteSlide : IDisposable
{
    private const double SalaryMin = 30_000;
    private const double SalaryMax = 200_000;

    // Debug builds use a smaller set to keep the dev loop quick; Release and the deployed deck show the full set.
#if DEBUG
    public const int RowCount = 10_000;
#else
    public const int RowCount = 50_000;
#endif

    private static readonly List<Employee> people = People.Generate(RowCount);
    private static readonly string[] departments = people.Select(p => p.Department).Distinct().Order().ToArray();
    private static readonly string[] optionalColumns = ["Title", "Country", "Rating", "Remote", "Hired"];

    private readonly Dictionary<string, List<IgbGridLiteFilterExpression>> filterState = [];
    private List<IgbGridLiteFilterExpression> filters = [];
    private List<IgbGridLiteSortingExpression> sortState = [];
    private List<IgbGridLiteSortingExpression> sorting = [];
    private IgbGridLiteSortingOptions sortingOptions = new() { Mode = GridLiteSortingMode.Single };
    private readonly HashSet<string> hidden = ["Hired"];
    private string density = "2";
    private bool loading = true;
    private int matchCount = people.Count;
    private CancellationTokenSource? countCts;

    // Handed over once the empty grid has rendered, so the slide transition starts immediately.
    private List<Employee> gridData = [];

    private void OnGridRendered()
    {
        gridData = people;
        loading = false;
    }

    // ---- side panel values, derived from the filter state ----

    private string SearchText => Expressions(nameof(Employee.Name))
        .FirstOrDefault(f => f.Condition == "contains")?.SearchTerm?.ToString() ?? "";

    private double SalaryLow => Number(Expressions(nameof(Employee.Salary)).FirstOrDefault(f => f.Condition == "greaterThanOrEqual"), SalaryMin);

    private double SalaryHigh => Number(Expressions(nameof(Employee.Salary)).FirstOrDefault(f => f.Condition == "lessThanOrEqual"), SalaryMax);

    private bool IsDeptSelected(string dept) => Expressions(nameof(Employee.Department))
        .Any(f => f.Condition == "equals" && string.Equals(f.SearchTerm?.ToString(), dept, StringComparison.OrdinalIgnoreCase));

    private IEnumerable<IgbGridLiteFilterExpression> Expressions(string key) =>
        filterState.TryGetValue(key, out var list) ? list : [];

    private static double Number(IgbGridLiteFilterExpression? expression, double fallback) =>
        expression?.SearchTerm is { } term && double.TryParse(term.ToString(), CultureInfo.InvariantCulture, out var value) ? value : fallback;

    private static string Money(double value) => value.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

    // ---- side panel → filter state ----

    private void SetSearch(string? text) =>
        SetFilter(nameof(Employee.Name), string.IsNullOrEmpty(text)
            ? []
            : [new() { Key = nameof(Employee.Name), Condition = "contains", SearchTerm = text }]);

    private void OnSalary(IgbRangeSliderValueEventArgs e)
    {
        List<IgbGridLiteFilterExpression> range = [];
        if (e.Detail.Lower > SalaryMin)
        {
            range.Add(new() { Key = nameof(Employee.Salary), Condition = "greaterThanOrEqual", SearchTerm = e.Detail.Lower });
        }

        if (e.Detail.Upper < SalaryMax)
        {
            range.Add(new() { Key = nameof(Employee.Salary), Condition = "lessThanOrEqual", SearchTerm = e.Detail.Upper });
        }

        SetFilter(nameof(Employee.Salary), range);
    }

    private void ToggleDept(string dept, bool on)
    {
        var selected = departments.Where(d => d == dept ? on : IsDeptSelected(d));
        SetFilter(nameof(Employee.Department), selected
            .Select(d => new IgbGridLiteFilterExpression
            {
                Key = nameof(Employee.Department),
                Condition = "equals",
                SearchTerm = d,
                Criteria = GridLiteFilterCriteria.Or
            })
            .ToList());
    }

    // ---- grid UI → filter / sort state ----

    private void OnFiltered(IgbGridLiteFilteredEventArgs e) => SetFilter(e.Key, [.. e.State], pushToGrid: false);

    private void OnSorted(IgbGridLiteSortedEventArgs e)
    {
        var others = sortingOptions.Mode == GridLiteSortingMode.Multiple
            ? sortState.Where(s => s.Key != e.Expression.Key)
            : [];
        sortState = e.Expression.Direction == GridLiteSortingDirection.None
            ? [.. others]
            : [.. others, e.Expression];
    }

    // ---- state ----

    private void SetFilter(string key, List<IgbGridLiteFilterExpression> expressions, bool pushToGrid = true)
    {
        if (expressions.Count == 0)
        {
            filterState.Remove(key);
        }
        else
        {
            filterState[key] = expressions;
        }

        if (pushToGrid)
        {
            filters = [.. filterState.Values.SelectMany(f => f)];
        }

        _ = UpdateMatchCountAsync();
    }

    // The header count is recomputed shortly after the last change, off the keystroke path.
    private async Task UpdateMatchCountAsync()
    {
        countCts?.Cancel();
        var cts = countCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(200, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        var matches = GridLiteMatch.Compile(filterState);
        matchCount = people.Count(matches);
        StateHasChanged();
    }

    private void ToggleColumn(string column, bool visible)
    {
        if (visible)
        {
            hidden.Remove(column);
        }
        else
        {
            hidden.Add(column);
        }
    }

    private void OnSortMode(IgbComponentValueChangedEventArgs e) =>
        sortingOptions = new() { Mode = e.Detail == "multiple" ? GridLiteSortingMode.Multiple : GridLiteSortingMode.Single };

    private void OnDensity(IgbComponentValueChangedEventArgs e) => density = e.Detail ?? "2";

    private void SortBySalary() =>
        sorting = sortState = [new() { Key = nameof(Employee.Salary), Direction = GridLiteSortingDirection.Descending }];

    private void ResetAll()
    {
        filterState.Clear();
        filters = [];
        sorting = sortState = [];
        countCts?.Cancel();
        matchCount = people.Count;
    }

    public void Dispose() => countCts?.Cancel();
}
