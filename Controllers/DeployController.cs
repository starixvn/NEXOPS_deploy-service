using DeploymentAPI.Models;
using DeploymentAPI.Services;
using Fanex.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class DeployController : ControllerBase
{
    private readonly ICommandService commandService;

    public DeployController(ICommandService commandService)
    {
        this.commandService = commandService;
    }

    [HttpPost]
    public IResult RemoveSwarmConfigs([FromForm] IFormCollection form)
    {
        var filter = NormalizeFormVariable(form, "configName");
        var listCommand = $"docker config ls --format {{{{.Name}}}} | grep -e {filter}*";

        var configs = commandService.Execute(listCommand);
        
        IResult result;
        
        if (configs.HasError())
        {
            result = Results.Content(LogError(form, configs));
        }
        else
        {
            var configNames = configs.Output.Replace("\r\n", "").Replace("\n", "");
            var removeCommand = $"docker config rm {configNames}";
            result = ResponeFromCommand(removeCommand, form);
        }
        
        return result;
    }

    private static string? NormalizeFormVariable(IFormCollection form, string name) => form[name].ToString().Replace(" ", string.Empty); 

    private IResult ResponeFromCommand(string command, IFormCollection form)
    {
        var result = commandService.Execute(command);

        var responseText = result.Output;

        if (result.HasError())
        {
            responseText = LogError(form, result);
        }

        return Results.Content(responseText);
    }

    private static string LogError(IFormCollection form, CommandResult result)
    {
        string responseText;
        var pipelineId = NormalizeFormVariable(form, "pipelineId");
        var host = NormalizeFormVariable(form, "host");
        var env = NormalizeFormVariable(form, "env");

        responseText = $"**[{env}|{host}|{pipelineId}]** {result.Error}\n- Output: {result.Output}";

        LogService.Error(responseText);
        return responseText;
    }

    [HttpPost]
    public IResult LoginRegistry([FromForm] IFormCollection form)
    {
        var user = NormalizeFormVariable(form, "registryUser");
        var password = NormalizeFormVariable(form, "registryPassword");
        var url = NormalizeFormVariable(form, "registryUrl");

        var command = $"docker login -u {user} -p {password} ${url}";

        return ResponeFromCommand(command, form);
    }

    [HttpPost]
    public IResult PrepareFolderPermission([FromForm] IFormCollection form)
    {
        var user = NormalizeFormVariable(form, "user");
        var workingDirectory = NormalizeFormVariable(form, "workingDirectory");
        var env = NormalizeFormVariable(form, "env");

        var command = $"mkdir -p {workingDirectory}/{env}/configs && chown ${user}:${user} -R ${workingDirectory} && chmod 755 -R ${workingDirectory}";

        return ResponeFromCommand(command, form);
    }

    [HttpPost]
    public IResult DeploySwarmService([FromForm] IFormCollection form)
    {
        var stack = NormalizeFormVariable(form, "stack");
        var workingDirectory = NormalizeFormVariable(form, "workingDirectory");
        var env = NormalizeFormVariable(form, "env");

        var command = $"docker stack deploy --compose-file={workingDirectory}/{env}/docker-compose.yml --with-registry-auth {stack}";

        return ResponeFromCommand(command, form);
    }

    [HttpPost]
    public IResult Execute([FromForm] IFormCollection form)
    {
        var command = form["command"].ToString();

        return ResponeFromCommand(command, form);
    }
}