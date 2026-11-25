using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

// IgbGroupingExpression is a plain settings object here (handed to GroupingExpressions / GroupByAsync), as the grid docs show.
#pragma warning disable BL0005

/// <summary>
/// The commercial IgbGrid on the same employees as the Grid Lite slide. The side panel drives the grid through its API
/// (GroupByAsync, SelectRowsAsync, …); the grid's own UI reports back through GroupingDone, RowSelectionChanging and
/// RowEditDone, so the chips and counters stay in step whichever side made the change.
/// </summary>
public partial class GridSlide
{
    public const int RowCount = 10_000;

    private static readonly List<Employee> people = People.Generate(RowCount);
    private static readonly string[] groupableFields = [nameof(Employee.Department), nameof(Employee.Country), nameof(Employee.Title)];

    // Initial state only: handed to the grid once and never reassigned; later changes go through the API.
    private static readonly IgbGroupingExpression[] initialGrouping =
        [new() { FieldName = nameof(Employee.Department), Dir = SortingDirection.Asc }];

    private static readonly object[] topEarners = people
        .OrderByDescending(p => p.Salary).ThenBy(p => p.Id)
        .Take(25)
        .Select(p => (object)p.Id)
        .ToArray();

    private IgbGrid grid = default!;
    private readonly HashSet<string> grouped = [nameof(Employee.Department)];
    private FilterMode filterMode = FilterMode.QuickFilter;
    private bool summaries = true;
    private int selectedCount;
    private int editCount;

    // Grouped: a summary per group (shown under collapsed groups too), no root row eating the viewport.
    // Ungrouped: the root summary is the only one there is.
    private GridSummaryCalculationMode SummaryMode =>
        grouped.Count > 0 ? GridSummaryCalculationMode.ChildLevelsOnly : GridSummaryCalculationMode.RootLevelOnly;

    private async Task ToggleGroupAsync(string field, bool on)
    {
        if (on)
        {
            grouped.Add(field);
            await grid.GroupByAsync([new IgbGroupingExpression { FieldName = field, Dir = SortingDirection.Asc }]);
        }
        else
        {
            grouped.Remove(field);
            await grid.ClearGroupingAsync(field);
        }
    }

    private Task ToggleGroupRowsAsync() => grid.ToggleAllGroupRowsAsync();

    // Grouping changed from the grid's UI (group area, column menu): mirror it in the chips.
    private void OnGroupingDone(IgbGroupingDoneEventArgs e)
    {
        grouped.Clear();
        foreach (var expression in e.Detail?.Expressions ?? [])
        {
            grouped.Add(expression.FieldName);
        }
    }

    private void OnFilterMode(IgbComponentValueChangedEventArgs e) =>
        filterMode = e.Detail == "excel" ? FilterMode.ExcelStyleFilter : FilterMode.QuickFilter;

    private async Task SelectTopEarnersAsync()
    {
        await grid.SelectRowsAsync(topEarners, true);
        selectedCount = topEarners.Length;
    }

    private async Task ClearSelectionAsync()
    {
        await grid.DeselectAllRowsAsync(false);
        selectedCount = 0;
    }

    private void OnRowSelectionChanging(IgbRowSelectionEventArgs e) =>
        selectedCount = e.Detail?.NewSelection?.Length ?? 0;

    private void OnRowEditDone(IgbGridEditDoneEventArgs e) => editCount++;
}
