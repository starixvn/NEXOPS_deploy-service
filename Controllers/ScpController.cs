using Fanex.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ScpController : ControllerBase
{
    [HttpPost]
    public IResult Post([FromForm] IFormCollection form)
    {
        IResult result = TypedResults.BadRequest();
        var file = form.Files?[0];
        var remotePath = form["remotePath"].ToString() ?? string.Empty;
        try
        {
            using var stream = System.IO.File.Create(remotePath);
            file?.CopyTo(stream);
            result = TypedResults.Ok();
        }
        catch (Exception ex)
        {
            LogService.Error($"**[{form["host"]}|{form["env"]}|{form["pipelineId"]}]** {ex.Message}", ex);
        }
        
        return result;
    }

}