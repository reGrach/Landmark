using System.Diagnostics;
using Landmark.Database;
using Landmark.Database.Enums;
using Landmark.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace Landmark.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LandmarkContext _context;

    public HomeController(ILogger<HomeController> logger, LandmarkContext ctx)
    {
        _logger = logger;
        _context = ctx;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _context.Races
            .Include(x => x.Participants)
            .OrderBy(x => x.StartTime)
            .Select(x => new RaceModel
            {
                Number = x.TeamNumber,
                SerialTag = x.SerialTag,
                CountPoints = x.CountPoints,
                StartTime = x.StartTime.HasValue ? x.StartTime.Value.ToString("HH:mm:ss") : string.Empty,
                FinishTime = x.FinishTime.HasValue ? x.FinishTime.Value.ToString("HH:mm:ss") : string.Empty,
                Participants = x.Participants.Select(y => y.Name).ToList(),
                SexType = x.SexType == SexType.Male
                ? "М"
                : x.SexType == SexType.Female ? "Ж" : "М/Ж",
            })
            .ToListAsync();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

