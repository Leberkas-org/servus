using Microsoft.Extensions.Configuration;

namespace Servus.Application.Startup;

public interface IConfigurationSetupContainer : ISetupContainer
{
    void SetupConfiguration(IConfigurationManager builder);
}