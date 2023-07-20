using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentAPI.Controllers;

[Authorize]
[ApiController]
[Route("/")]
public class HealthCheckController: ControllerBase
{
    [HttpGet]
    public IResult HealthCheck()
    {
        return Results.Content($"STATUS: {Response.StatusCode}");
    }
}