using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DeploymentAPI.CustomMiddlewares.CustomAuthentication;

public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
{
    private readonly IConfiguration configuration;

    public CustomAuthenticationHandler(IOptionsMonitor<CustomAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        this.configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult result;
        if (!Request.Headers.ContainsKey("DEPLOYMENT-API-TOKEN"))
        {
            result = AuthenticateResult.Fail("Missing Token!");
        }
        else
        {
            var token = Request?.Headers["DEPLOYMENT-API-TOKEN"] ?? string.Empty;

            if (token != configuration["DeploymentApiToken"])
            {
                result = AuthenticateResult.Fail($"Token is invalid!");
            }
            else
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "GitLab-CICD")
                };

                var claimsIdentity = new ClaimsIdentity
                    (claims, this.Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal
                    (claimsIdentity);

                result = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, this.Scheme.Name));
            }
        }

        return Task.FromResult(result);
    }
}
