namespace DeploymentAPI.Models;

public class PrepareFolderPermissionViewModel : BaseViewModel
{
    public string User { get; set; } = string.Empty;

    public string WorkingDirectory { get; set; } = string.Empty;
}