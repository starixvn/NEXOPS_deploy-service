namespace DeploymentAPI.Models;

public class DeploySwarmServiceViewModel: BaseViewModel
{
    public string Stack { get; set; } = string.Empty;

    public string WorkingDirectory { get; set; } = string.Empty;
}