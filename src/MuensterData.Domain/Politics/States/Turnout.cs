namespace MuensterData.Domain.Politics.States;

public record Turnout(int Eligible, int Actual)
{
    public decimal Percentage = Actual / (decimal)Eligible * 100;
    public IReadOnlyCollection<object> FormatForChart =
    [
        new
        {
            Name = "Gewählt",
            Count = (Actual / (decimal)Eligible * 100),
            Label = $"Gewählt: {(Actual /(decimal) Eligible * 100):##.00}%",
            Fill = "#007a99"
        },
        new
        {
            Name = "Nicht gewählt",
            Count = 100 - (Actual / (decimal)Eligible * 100),
            Label = $"Nicht gewählt: {100 - (Actual /(decimal) Eligible * 100):##.00}%",
            Fill = "#c4c91f"
        }
    ];
};