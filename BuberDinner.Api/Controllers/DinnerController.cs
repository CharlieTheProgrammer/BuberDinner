using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("dinners")]
public class DinnerController : ApiController
{
    [HttpGet]
    public IActionResult GetDinners()
    {
        return Ok();
    }
}
