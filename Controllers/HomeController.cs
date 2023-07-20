using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentAPI.Controllers;

[Authorize]
[ApiController]
[Route("/home")]
public class HomeController: ControllerBase
{
    [HttpGet]
    public IResult Get()
    {
        return Results.Content("Deployment API is running!");
    }

    [HttpGet("whoami")]
    public IResult WhoAmI()
    {
        var ip = Request.Headers["x-forwarded-for"].ToString().Split(',').FirstOrDefault();

        return Results.Content($"Hello! It's me {(string.IsNullOrEmpty(ip) ? Request.HttpContext.Connection.RemoteIpAddress : ip)}");
    }

    [HttpGet("testerror")]
    public IResult Test() => throw new InvalidOperationException("Test Log Sentry");

}