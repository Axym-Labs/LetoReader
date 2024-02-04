using System;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class GreetingController : ControllerBase
{
    [HttpGet("{name}")]
    public IActionResult GetGreetingFromUrlRoute(string name)
    {
        string greeting = $"Hello, {name}";
        return Ok(new { Greeting = greeting });
    }

    [HttpGet]
    public IActionResult GetGreetingFromQueryArg([FromQuery] string name)
    {
        string greeting = $"Hello, {name}!";
        return Ok(new { Greeting = greeting });
    }
    [HttpPost]
    public IActionResult GetGreetingFromRequestBody([FromBody] dynamic data)
    {
        if (string.IsNullOrEmpty(data?.name))
        {
            return BadRequest("Name is required.");
        }
        
        string? name = data?.name;

        string greeting = $"Hello, {name}";
        return Ok(new { Greeting = greeting });
    }
}