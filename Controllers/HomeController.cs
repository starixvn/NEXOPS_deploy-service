using Fanex.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentAPI.Controllers;

[Authorize]
[ApiController]
[Route("/")]
public class HomeController: ControllerBase
{
    [HttpGet]
    public IResult Get()
    {
        return Results.Content($"- Deployment API is running!\n- Status: {Response.StatusCode}");
    }

    [HttpGet("whoami")]
    public IResult WhoAmI()
    {
        var ip = Request.Headers["x-forwarded-for"].ToString().Split(',').FirstOrDefault();

        return Results.Content($"Hello! It's me {(string.IsNullOrEmpty(ip) ? Request.HttpContext.Connection.RemoteIpAddress : ip)}");
    }

    [HttpGet("testlogsentry")]
    public IResult Test([FromForm] string log) => throw new InvalidOperationException($"Test Log Sentry: {log}");

    [Route("/error")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
 
        var exception = context?.Error;
 
        LogService.Error(exception?.Message, exception);
 
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Detail = exception?.Message
        };
 
        return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
    }
}