using System.Globalization;
using IgniteUI.Blazor.Controls;
using StandupDemo.Data;

namespace StandupDemo.Slides;

/// <summary>
/// Compiles Grid Lite filter expressions into a .NET predicate, so the slide can count the rows the
/// grid's current filters match. Per column: any "or" expression matching passes, otherwise every
/// "and" expression must match; columns combine with "and", the same as the grid.
/// </summary>
public static class GridLiteMatch
{
    public static Func<Employee, bool> Compile(IReadOnlyDictionary<string, List<IgbGridLiteFilterExpression>> state)
    {
        var columns = state.Select(column => CompileColumn(column.Key, column.Value)).ToArray();
        return columns.Length == 0 ? _ => true : row => Array.TrueForAll(columns, column => column(row));
    }

    private static Func<Employee, bool> CompileColumn(string key, List<IgbGridLiteFilterExpression> expressions)
    {
        var ors = expressions.Where(e => e.Criteria == GridLiteFilterCriteria.Or).Select(e => CompileExpression(key, e)).ToArray();
        var ands = expressions.Where(e => e.Criteria != GridLiteFilterCriteria.Or).Select(e => CompileExpression(key, e)).ToArray();

        if (ands.Length == 0)
        {
            return ors.Length == 0 ? _ => true : row => Array.Exists(ors, test => test(row));
        }

        return row => Array.Exists(ors, test => test(row)) || Array.TrueForAll(ands, test => test(row));
    }

    private static Func<Employee, bool> CompileExpression(string key, IgbGridLiteFilterExpression e)
    {
        var term = e.SearchTerm?.ToString() ?? "";

        switch (key)
        {
            case nameof(Employee.Remote):
                return e.Condition switch
                {
                    "true" => row => row.Remote,
                    "false" => row => !row.Remote,
                    _ => _ => true
                };

            case nameof(Employee.Id) or nameof(Employee.Salary) or nameof(Employee.Rating):
                Func<Employee, double> number = key switch
                {
                    nameof(Employee.Id) => row => row.Id,
                    nameof(Employee.Salary) => row => row.Salary,
                    _ => row => row.Rating
                };
                if (!double.TryParse(term, CultureInfo.InvariantCulture, out var target))
                {
                    return _ => true;
                }

                return e.Condition switch
                {
                    "equals" => row => number(row) == target,
                    "doesNotEqual" => row => number(row) != target,
                    "greaterThan" => row => number(row) > target,
                    "lessThan" => row => number(row) < target,
                    "greaterThanOrEqual" => row => number(row) >= target,
                    "lessThanOrEqual" => row => number(row) <= target,
                    _ => _ => true
                };

            default:
                Func<Employee, string> text = key switch
                {
                    nameof(Employee.Name) => row => row.Name,
                    nameof(Employee.Title) => row => row.Title,
                    nameof(Employee.Department) => row => row.Department,
                    nameof(Employee.Country) => row => row.Country,
                    nameof(Employee.Hired) => row => row.Hired,
                    _ => _ => ""
                };
                var comparison = e.CaseSensitive == true ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return e.Condition switch
                {
                    "contains" => row => text(row).Contains(term, comparison),
                    "doesNotContain" => row => !text(row).Contains(term, comparison),
                    "startsWith" => row => text(row).StartsWith(term, comparison),
                    "endsWith" => row => text(row).EndsWith(term, comparison),
                    "equals" => row => string.Equals(text(row), term, comparison),
                    "doesNotEqual" => row => !string.Equals(text(row), term, comparison),
                    "empty" => row => text(row).Length == 0,
                    "notEmpty" => row => text(row).Length > 0,
                    _ => _ => true
                };
        }
    }
}
