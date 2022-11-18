using BeerSender.Domain;
using BeerSender.Domain.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.API.Controllers;

[Route("[controller]")]
[ApiController]
public class CommandController : ControllerBase
{
    private readonly Command_router _router;

    public CommandController(Command_router router)
    {
        _router = router;
    }

    [HttpPost]
    public IActionResult PostCommand(ICommand command)
    {
        var cmd2 = (Create_package.Command)command;
        _router.Handle_command<Create_package.Command, Beer_package>(cmd2);
        return Accepted();
    }
}