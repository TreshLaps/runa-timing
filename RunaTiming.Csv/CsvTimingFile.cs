using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace RunaTiming.Csv;

public class CsvTimingFile
{
    public int Bib { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }

    public Sex Sex { get; set; }

    [Format("yyyy.MM.dd", "dd.MM.yyyy")] public DateTime BirthDate { get; set; }
    public string RaceName { get; set; }

    [Format("yyyy.MM.dd HH:mm:ss.fff", "dd.MM.yyyy HH:mm:ss.fff")]
    public DateTime? StartTime { get; set; }

    public DateTime? ChipStartTime { get; set; }
    public string FinishingTime { get; set; }
    public string Splits { get; set; }

    public bool IsNewerThan(CsvTimingFile otherItem)
    {
        return $"{StartTime}_{ChipStartTime}_{FinishingTime}_{Splits}".Length >=
               $"{otherItem.StartTime}_{otherItem.ChipStartTime}_{otherItem.FinishingTime}_{otherItem.Splits}".Length;
    }
}

public enum Sex
{
    [Name("Male")] Male = 0,
    [Name("Female")] Female = 1
}

public sealed class CsvTimingFileMap : ClassMap<CsvTimingFile>
{
    public CsvTimingFileMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.Bib).Name("Bib #");
        Map(m => m.LastName).Name("Last Name");
        Map(m => m.FirstName).Name("First Name");
        Map(m => m.BirthDate).Name("DOB");
        Map(m => m.RaceName).Name("Race Name");
        Map(m => m.StartTime).Name("Race Start Date/Time");
        Map(m => m.ChipStartTime).Name("Chip Start Date/Time");
        Map(m => m.FinishingTime).Name("Finishing Time");
        Map(m => m.Splits).Name("Split Times (by chip time)");
    }
}