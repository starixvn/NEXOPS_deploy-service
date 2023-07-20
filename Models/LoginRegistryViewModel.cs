namespace DeploymentAPI.Models;

public class LoginRegistryViewModel : BaseViewModel
{
    public string RegistryUser { get; set; } = string.Empty;

    public string RegistryPassword { get; set; } = string.Empty;

    public string RegistryUrl { get; set; } = string.Empty;
}