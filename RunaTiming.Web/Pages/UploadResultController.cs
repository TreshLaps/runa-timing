using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RunaTiming.Db;
using RunaTiming.Races;
using RunaTiming.Shared.Upload;

namespace RunaTiming.Web.Pages
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadResultController : Controller
    {
        private readonly RaceService _raceService;
        private readonly DataContext _dataContext;

        public UploadResultController(RaceService raceService, DataContext dataContext)
        {
            _raceService = raceService;
            _dataContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<ResultItem> results)
        {
            var resultGroups = results
                .GroupBy(result => result.RaceName);

            foreach (var resultGroup in resultGroups)
            {
                var raceName = resultGroup.Key;
                var race = _dataContext.Races.FirstOrDefault(race => race.Name == raceName);
                var bibs = resultGroup.Select(result => result.Bib).ToArray();

                if (race == null)
                {
                    return BadRequest(new { Message = $"Race \"{raceName}\" was not found in the database" });
                }

                var dbResults = _dataContext.Results
                    .Where(result => result.RaceId == race.Id && bibs.Contains(result.Bib))
                    .ToList();

                foreach (var resultItem in resultGroup)
                {
                    var dbResult = dbResults.FirstOrDefault(r => r.Bib == resultItem.Bib);

                    if (dbResult == null)
                    {
                        dbResult = new Db.Models.Result
                        {
                            Bib = resultItem.Bib,
                            RaceId = race.Id
                        };

                        _dataContext.Results.Add(dbResult);
                    }

                    dbResult.Modified = System.DateTime.UtcNow;
                    dbResult.FirstName = resultItem.FirstName;
                    dbResult.LastName = resultItem.LastName;
                    dbResult.BirthDate = resultItem.BirthDate;
                    dbResult.StartTime = resultItem.StartTime;
                    dbResult.ChipStartTime = resultItem.ChipStartTime;
                    dbResult.FinishingTime = resultItem.FinishingTime;
                    dbResult.Splits = resultItem.Splits;
                }
            }

            await _dataContext.SaveChangesAsync();
            return Ok();
        }
    }
}