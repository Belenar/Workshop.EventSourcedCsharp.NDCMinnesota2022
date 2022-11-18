using BeerSender.API.Read_models;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.API.Controllers;

[Route("[controller]")]
[ApiController]
public class BeerPackageController : ControllerBase
{
    private readonly Read_context _db;

    public BeerPackageController(Read_context db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Package_beer>> GetBeer(Guid packageId)
    {
        return Ok(_db.Package_beers.Where(p => p.PackageId == packageId).ToList());
    }
}