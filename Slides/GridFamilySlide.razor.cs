using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

/// <summary>Tree Grid, Hierarchical Grid and Pivot Grid, one at a time, on data derived from the deck's employees.</summary>
public partial class GridFamilySlide
{
    private const int PeopleCount = 3_000;

    private static readonly Dictionary<string, string> Captions = new()
    {
        ["tree"] = "Flat rows with a ManagerId become an org chart: ForeignKey does the nesting. Cascading selection, and filtering that keeps each match's ancestors.",
        ["hierarchical"] = "Country, then department, then people: each level is a row island with its own columns, sorting and selection.",
        ["pivot"] = $"Average salary by department and country over {PeopleCount:N0} employees. Drag dimensions in the selector, or change the aggregate from the value chip.",
    };

    private static readonly List<Employee> pivotData = People.Generate(PeopleCount);
    private static readonly List<OrgMember> orgChart = Org.Chart();
    private static readonly List<CountryRow> byCountry = Org.ByCountry(pivotData);

    // The pivot configuration objects are plain settings, built once in code as the Pivot Grid docs show.
#pragma warning disable BL0005
    private static readonly IgbPivotConfiguration pivotConfig = new()
    {
        Rows = [new() { MemberName = nameof(Employee.Department), Enabled = true }],
        Columns = [new() { MemberName = nameof(Employee.Country), Enabled = true }],
        Filters = [new() { MemberName = nameof(Employee.Title), Enabled = true }],
        Values =
        [
            new()
            {
                Member = nameof(Employee.Salary),
                DisplayName = "Salary",
                DataType = GridColumnDataType.Currency,
                FormatterScript = "PivotCurrency", // wwwroot/js/templates.js: whole dollars
                Enabled = true,
                Aggregate = Aggregator(PivotAggregationType.AVG, "Average"),
                AggregateList =
                [
                    Aggregator(PivotAggregationType.AVG, "Average"),
                    Aggregator(PivotAggregationType.SUM, "Sum"),
                    Aggregator(PivotAggregationType.MIN, "Minimum"),
                    Aggregator(PivotAggregationType.MAX, "Maximum"),
                ],
            },
        ],
    };

    private static IgbPivotAggregator Aggregator(PivotAggregationType type, string label) =>
        new() { AggregatorName = type, Key = type.ToString(), Label = label };
#pragma warning restore BL0005

    private string view = "tree";

    // The data selector takes the grid instance, which only exists after the grid has rendered.
    private IgbPivotGrid? pivotGrid;
    private IgbPivotGrid? PivotGrid
    {
        get => pivotGrid;
        set
        {
            if (!ReferenceEquals(pivotGrid, value))
            {
                pivotGrid = value;
                StateHasChanged();
            }
        }
    }

    private void OnView(IgbComponentValueChangedEventArgs e)
    {
        view = e.Detail is "hierarchical" or "pivot" ? e.Detail : "tree";
        if (view != "pivot")
        {
            pivotGrid = null;
        }
    }
}
