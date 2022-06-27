using RunaTiming.Db;
using RunaTiming.Db.Models;

namespace RunaTiming.Races;

public class RaceService
{
    private readonly DataContext _dataContext;

    public RaceService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public List<Race> ListRaces()
    {
        return _dataContext.Races
            .OrderByDescending(race => race.Date)
            .ToList();
    }
}