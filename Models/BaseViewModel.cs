namespace DeploymentAPI.Models;

public class BaseViewModel 
{
    public string PipelineId { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string Env { get; set; } = string.Empty;
}