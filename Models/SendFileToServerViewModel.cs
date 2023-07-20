namespace DeploymentAPI.Models;

public class SendFileToServerViewModel: BaseViewModel
{
    public string RemotePath { get; set; } = string.Empty;

    public IFormFile File { get; set; } = null!;
}