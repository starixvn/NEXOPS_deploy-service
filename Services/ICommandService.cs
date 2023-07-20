using DeploymentAPI.Models;

namespace DeploymentAPI.Services;

public interface ICommandService
{
    CommandResult Execute(string command);
}