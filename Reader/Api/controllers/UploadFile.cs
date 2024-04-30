using System;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/upload/[controller]")]
public class GreetingController : ControllerBase
{

    [HttpPost]
    public IActionResult UploadFile([FromBody] dynamic data)
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