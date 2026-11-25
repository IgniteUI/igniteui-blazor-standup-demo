namespace StandupDemo.Data;

/// <summary>A flat org-chart row: <see cref="ManagerId"/> points at the parent row (null for the root).</summary>
public sealed class OrgMember
{
    public int Id { get; set; }
    public int? ManagerId { get; set; }
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Department { get; set; } = "";
    public string Country { get; set; } = "";
    public int Salary { get; set; }
}

/// <summary>Root level of the hierarchical grid: one row per country, with its departments nested.</summary>
public sealed class CountryRow
{
    public string Country { get; set; } = "";
    public int Headcount { get; set; }
    public int AvgSalary { get; set; }
    public List<DepartmentRow> Departments { get; set; } = [];
}

/// <summary>Second level: a department within a country, with its people nested.</summary>
public sealed class DepartmentRow
{
    public string Key { get; set; } = "";
    public string Department { get; set; } = "";
    public int Headcount { get; set; }
    public int AvgSalary { get; set; }
    public double AvgRating { get; set; }
    public List<Employee> People { get; set; } = [];
}

/// <summary>Deterministic org and hierarchy data built from <see cref="People"/>.</summary>
public static class Org
{
    private static readonly (string Department, string Vp, string[] Teams)[] Divisions =
    [
        ("Engineering", "VP Engineering", ["Grids", "Charts", "Blazor"]),
        ("Product", "VP Product", ["Design", "Research", "Docs"]),
        ("Sales", "VP Sales", ["EMEA", "Americas", "APAC"]),
        ("DevRel", "VP Developer Relations", ["Content", "Community", "Events"]),
    ];

    /// <summary>CEO, four VPs, three leads per VP and four people per lead: 65 rows, flat, linked by ManagerId.</summary>
    public static List<OrgMember> Chart()
    {
        var names = People.Generate(80, seed: 7);
        var rnd = new Random(7);
        var rows = new List<OrgMember>();
        var id = 0;

        OrgMember Add(int? managerId, string title, string department, int salary)
        {
            var person = names[id];
            var row = new OrgMember
            {
                Id = ++id,
                ManagerId = managerId,
                Name = person.Name,
                Title = title,
                Department = department,
                Country = person.Country,
                Salary = salary + rnd.Next(0, 20) * 1_000,
            };
            rows.Add(row);
            return row;
        }

        var ceo = Add(null, "CEO", "Executive", 240_000);
        foreach (var (department, vpTitle, teams) in Divisions)
        {
            var vp = Add(ceo.Id, vpTitle, department, 180_000);
            foreach (var team in teams)
            {
                var lead = Add(vp.Id, $"{team} Lead", department, 120_000);
                for (var i = 0; i < 4; i++)
                {
                    Add(lead.Id, i == 0 ? "Senior Engineer" : "Engineer", department, i == 0 ? 95_000 : 70_000);
                }
            }
        }

        return rows;
    }

    /// <summary>Country → department → employee, each level with its own columns.</summary>
    public static List<CountryRow> ByCountry(IEnumerable<Employee> people) =>
        people
            .GroupBy(p => p.Country)
            .OrderBy(g => g.Key)
            .Select(country => new CountryRow
            {
                Country = country.Key,
                Headcount = country.Count(),
                AvgSalary = (int)country.Average(p => p.Salary),
                Departments = country
                    .GroupBy(p => p.Department)
                    .OrderBy(g => g.Key)
                    .Select(dept => new DepartmentRow
                    {
                        Key = $"{country.Key}/{dept.Key}",
                        Department = dept.Key,
                        Headcount = dept.Count(),
                        AvgSalary = (int)dept.Average(p => p.Salary),
                        AvgRating = Math.Round(dept.Average(p => p.Rating), 2),
                        People = [.. dept.OrderBy(p => p.Name)],
                    })
                    .ToList(),
            })
            .ToList();
}
