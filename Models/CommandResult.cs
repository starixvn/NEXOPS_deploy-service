namespace DeploymentAPI.Models;

public class CommandResult
{
    public string Output {get;set;} = string.Empty;

    public string Error {get;set;} = string.Empty;

    public bool HasError() => !string.IsNullOrEmpty(Error);
}