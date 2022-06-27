using RunaTiming.Db;
using RunaTiming.Db.Models;

namespace RunaTiming.Races;

public class RaceResultService
{
    private readonly DataContext _dataContext;

    public RaceResultService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public List<Result> ListResults(int raceId)
    {
        return _dataContext.Results
            .Where(r => r.RaceId == raceId)
            .OrderByDescending(r => r.Bib)
            .ToList();
    }
}