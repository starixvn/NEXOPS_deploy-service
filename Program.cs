using DeploymentAPI.CustomMiddlewares.CustomAuthentication;
using DeploymentAPI.Services;
using Fanex.Logging;
using Fanex.Logging.Sentry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CustomAuthenticationOptions.DefaultScheme)
                .AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>(CustomAuthenticationOptions.DefaultScheme, opt => {});
builder.Services.AddControllers();
builder.Services.AddSingleton<ICommandService, CommandService>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

LogManager.SetDefaultLogCategory(builder.Configuration["Sentry:Category"])
            .Use(new SentryLogging(new SentryEngineOptions {Dsn = new Sentry.Dsn(builder.Configuration["Sentry:Dsn"])
}));

app.UseExceptionHandler("/error");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
