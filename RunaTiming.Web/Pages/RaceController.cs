using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RunaTiming.Db.Models;
using RunaTiming.Races;

namespace RunaTiming.Web.Pages;

[ApiController]
[Route("api/[controller]")]
public class RaceController : Controller
{
    private readonly RaceService _raceService;
    private readonly RaceResultService _raceResultService;

    public RaceController(RaceService raceService, RaceResultService raceResultService)
    {
        _raceService = raceService;
        _raceResultService = raceResultService;
    }

    [HttpGet]
    public List<Race> List()
    {
        return _raceService.ListRaces();
    }

    [HttpGet("{raceId}/results")]
    public List<Result> List(int raceId)
    {
        return _raceResultService.ListResults(raceId);
    }
}