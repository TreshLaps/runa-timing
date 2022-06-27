namespace RunaTiming.Shared.Upload;

public class ResultItem
{
    public int Bib { get; set; }
    public string RaceName { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }

    public DateTime? StartTime { get; set; }
    public double? ChipStartTime { get; set; }
    public double? FinishingTime { get; set; }
    public List<double> Splits { get; set; } = new();
}