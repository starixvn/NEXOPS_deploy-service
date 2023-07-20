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
    public IResult RemoveSwarmConfigs([FromForm] SwarmConfigViewModel model)
    {
        var filter = model.ConfigName.Replace(" ", string.Empty);
        var listCommand = $"docker config ls --format {{{{.Name}}}} | grep -e {filter}*";

        var configs = commandService.Execute(listCommand);

        IResult result;

        if (configs.HasError())
        {
            result = Results.Content(LogError(model, configs));
        }
        else
        {
            var configNames = configs.Output.Replace("\r\n", "").Replace("\n", "");
            var removeCommand = $"docker config rm {configNames}";
            result = ResponeFromCommand(removeCommand, model);
        }

        return result;
    }

    private static string NormalizeText(string text) => text.Replace(" ", string.Empty);

    private IResult ResponeFromCommand(string command, BaseViewModel model)
    {
        var result = commandService.Execute(command);

        var responseText = result.Output;

        if (result.HasError())
        {
            responseText = LogError(model, result);
        }

        return Results.Content(responseText);
    }

    private static string LogError(BaseViewModel model, CommandResult result)
    {
        var responseText = $"**[{model.Env}|{model.Host}|{model.PipelineId}]** {result.Error}\n- Output: {result.Output}";

        LogService.Error(responseText);
        return responseText;
    }

    [HttpPost]
    public IResult LoginRegistry([FromForm] LoginRegistryViewModel model)
    {
        var user = NormalizeText(model.RegistryUser);
        var password = NormalizeText(model.RegistryPassword);
        var url = NormalizeText(model.RegistryUrl);

        var command = $"docker login -u {user} -p {password} ${url}";

        return ResponeFromCommand(command, model);
    }

    [HttpPost]
    public IResult PrepareFolderPermission([FromForm] PrepareFolderPermissionViewModel model)
    {
        var user = NormalizeText(model.User);
        var workingDirectory = NormalizeText(model.WorkingDirectory);
        var env = NormalizeText(model.Env);

        var command = $"mkdir -p {workingDirectory}/{env}/configs && chown ${user}:${user} -R ${workingDirectory} && chmod 755 -R ${workingDirectory}";

        return ResponeFromCommand(command, model);
    }

    [HttpPost]
    public IResult DeploySwarmService([FromForm] DeploySwarmServiceViewModel model)
    {
        var workingDirectory = NormalizeText(model.WorkingDirectory);
        var stack = NormalizeText(model.Stack);
        var env = NormalizeText(model.Env);

        var command = $"docker stack deploy --compose-file={workingDirectory}/{env}/docker-compose.yml --with-registry-auth {stack}";

        return ResponeFromCommand(command, model);
    }

    [HttpPost]
    public IResult ExecuteCommand([FromForm] BaseViewModel model, [FromForm] string command)
        => ResponeFromCommand(command, model);

    [HttpPost]
    public IResult SendFileToServer([FromForm] SendFileToServerViewModel model)
    {
        IResult result = TypedResults.BadRequest();
        try
        {
            using var stream = System.IO.File.Create(model.RemotePath);
            model.File?.CopyTo(stream);
            result = TypedResults.Ok();
        }
        catch (Exception ex)
        {
            LogService.Error($"**[{model.Host}|{model.Env}|{model.PipelineId}]** {ex.Message}", ex);
        }

        return result;
    }
}