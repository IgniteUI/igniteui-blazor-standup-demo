namespace StandupDemo.Data;

public sealed class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Title { get; set; } = "";
    public string Department { get; set; } = "";
    public string Country { get; set; } = "";
    public int Salary { get; set; }
    public double Rating { get; set; }
    public bool Remote { get; set; }
    public string Hired { get; set; } = "";
}

/// <summary>Deterministic fake data so the demo looks the same every run.</summary>
public static class People
{
    private static readonly string[] First = ["Ava", "Liam", "Mia", "Noah", "Zoe", "Ethan", "Ivy", "Lucas", "Nora", "Leo", "Maya", "Owen", "Ella", "Kai", "Iris", "Theo", "Lena", "Max", "Sara", "Dan", "Elena", "Petar", "Yuki", "Omar", "Priya", "Jonas", "Chloe", "Mateo", "Aisha", "Viktor"];
    private static readonly string[] Last = ["Petrov", "Smith", "Garcia", "Chen", "Müller", "Rossi", "Kowalski", "Novak", "Silva", "Ivanova", "Johnson", "Tanaka", "Haddad", "Patel", "Nielsen", "Dubois", "Kim", "Nguyen", "Popescu", "Andersen"];
    private static readonly string[] Departments = ["Engineering", "Design", "Product", "Sales", "Marketing", "Support", "Finance", "DevRel"];
    private static readonly string[] Titles = ["Engineer", "Senior Engineer", "Lead", "Manager", "Director", "Architect", "Specialist", "Intern"];
    private static readonly string[] Countries = ["Bulgaria", "USA", "Germany", "Japan", "India", "Brazil", "UK", "Canada", "Spain", "Australia"];

    public static List<Employee> Generate(int count, int seed = 42)
    {
        var rnd = new Random(seed);
        var start = new DateTime(2012, 1, 1);
        var list = new List<Employee>(count);
        for (var i = 1; i <= count; i++)
        {
            var title = Titles[rnd.Next(Titles.Length)];
            var baseSalary = title switch
            {
                "Intern" => 30_000,
                "Director" => 160_000,
                "Architect" => 140_000,
                "Manager" => 120_000,
                "Lead" => 115_000,
                "Senior Engineer" => 100_000,
                _ => 70_000
            };
            list.Add(new Employee
            {
                Id = i,
                Name = $"{First[rnd.Next(First.Length)]} {Last[rnd.Next(Last.Length)]}",
                Title = title,
                Department = Departments[rnd.Next(Departments.Length)],
                Country = Countries[rnd.Next(Countries.Length)],
                Salary = baseSalary + rnd.Next(0, 40) * 1_000,
                Rating = Math.Round(2.5 + rnd.NextDouble() * 2.5, 1),
                Remote = rnd.Next(3) == 0,
                Hired = start.AddDays(rnd.Next(0, 5000)).ToString("yyyy-MM-dd"),
            });
        }

        return list;
    }
}
