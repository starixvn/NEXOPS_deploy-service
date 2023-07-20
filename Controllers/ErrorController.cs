using Fanex.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentAPI.Controllers;

public class ErrorController : ControllerBase
{
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