using DeploymentAPI.Models;

namespace DeploymentAPI.Services;

public class CommandService: ICommandService
{
    const string DEFAULTIMEOUT = "1000000";
    private readonly IConfiguration configuration;

    public CommandService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public CommandResult Execute(string command)
    {
        CommandResult commandResult= new();

        using System.Diagnostics.Process proc = new();
        proc.StartInfo.FileName = configuration["ShellPath"];
        proc.StartInfo.Arguments = "-c \" " + command + " \"";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.Start();

        commandResult.Output += proc.StandardOutput.ReadToEnd();
        commandResult.Error += proc.StandardError.ReadToEnd();

        proc.WaitForExit(Int32.Parse(configuration["ExecutionTimeout"] ?? DEFAULTIMEOUT));

        return commandResult;
    }
}