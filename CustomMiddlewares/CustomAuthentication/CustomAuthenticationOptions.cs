using Microsoft.AspNetCore.Authentication;

namespace DeploymentAPI.CustomMiddlewares.CustomAuthentication;

public class CustomAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "DeploymentApiAuthenticationScheme";
    public string TokenHeaderName { get; set; } = "DEPLOYMENT-API-TOKEN";
}