using BeerSender.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.API.Controllers;

[Route("[controller]")]
[ApiController]
public class CommandController : ControllerBase
{
   private readonly CommandRouter _commandRouter;

   public CommandController(CommandRouter commandRouter)
   {
      _commandRouter = commandRouter;
   }
   
   [HttpPost]
   public IActionResult PostCommand(Command command)
   {
      _commandRouter.HandleCommand(command);
      
      return Accepted();
   }
}