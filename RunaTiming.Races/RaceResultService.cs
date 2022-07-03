using RunaTiming.Db;
using RunaTiming.Db.Models;
using RunaTiming.Shared.ClassConverters;

namespace RunaTiming.Races;

public class RaceResultService
{
    private readonly DataContext _dataContext;

    public RaceResultService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public List<RaceResult> ListResults(int raceId)
    {
        var race = _dataContext.Races
            .First(race => race.Id == raceId);

        var dbResults = _dataContext.Results
            .Where(r => r.RaceId == raceId)
            .OrderByDescending(r => r.Bib)
            .ToList();

        var results = dbResults
            .Select(
                dbResult => new RaceResult
                {
                    Bib = dbResult.Bib,
                    RaceId = dbResult.RaceId,
                    Modified = dbResult.Modified,
                    FirstName = dbResult.FirstName,
                    LastName = dbResult.LastName,
                    Sex = dbResult.Sex.ToString(),
                    Class = GetClass(race.ClassType, dbResult.Sex, dbResult.BirthDate),
                    BirthDate = dbResult.BirthDate,
                    StartTime = dbResult.StartTime,
                    Finish = dbResult.FinishingTime.HasValue
                        ? new ResultTime
                        {
                            DistanceInMeters = race.Distance,
                            DurationInSeconds = dbResult.FinishingTime.Value
                        }
                        : null
                })
            .ToList();

        CalculatePosition(
            results,
            r => r.Finish != null,
            r => r.Finish.DurationInSeconds,
            (r, position) => { r.Finish.PositionOverall = position; });

        CalculatePosition(
            results,
            r => r.Finish != null && r.Sex == Sex.Male,
            r => r.Finish.DurationInSeconds,
            (r, position) => { r.Finish.PositionSex = position; });

        CalculatePosition(
            results,
            r => r.Finish != null && r.Sex == Sex.Female,
            r => r.Finish.DurationInSeconds,
            (r, position) => { r.Finish.PositionSex = position; });

        var athleteClasses = results
            .Select(r => r.Class)
            .ToHashSet();

        foreach (var athleteClass in athleteClasses)
        {
            CalculatePosition(
                results,
                r => r.Finish != null && r.Class == athleteClass,
                r => r.Finish.DurationInSeconds,
                (r, position) => { r.Finish.PositionClass = position; });
        }

        return results
            .OrderByDescending(r => r.Finish != null)
            .ThenBy(r => r.Finish?.PositionOverall)
            .ToList();
    }

    private string GetClass(ClassType classType, Sex athleteSex, DateTime? athleteBirthDate)
    {
        if (classType == ClassType.Kondis2022 && athleteBirthDate.HasValue)
        {
            return Kondis2022ClassConverter.Convert(athleteSex == Sex.Male, athleteBirthDate.Value);
        }

        return string.Empty;
    }

    private void CalculatePosition(
        List<RaceResult> results,
        Func<RaceResult, bool> whereFunc,
        Func<RaceResult, double> getDurationFunc,
        Action<RaceResult, int> setPosition)
    {
        var position = 1;

        foreach (var result in results.Where(whereFunc).OrderBy(getDurationFunc))
        {
            setPosition(result, position);
            position++;
        }
    }
}

public class RaceResult
{
    public int Bib { get; set; }

    public int RaceId { get; set; }

    public DateTime Modified { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Sex { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Class { get; set; }

    public DateTime? StartTime { get; set; }
    public double? ChipStartTime { get; set; }
    public ResultTime? Finish { get; set; }
    public List<RaceResultSplit> Splits { get; set; } = new();
}

public class RaceResultSplit
{
    public string Name { get; set; }
    public ResultTime SplitTime { get; set; }
    public ResultTime TotalTime { get; set; }
}

public class ResultTime
{
    public double DurationInSeconds { get; set; }
    public double DistanceInMeters { get; set; }

    public int PositionOverall { get; set; }
    public int PositionClass { get; set; }
    public int PositionSex { get; set; }
}