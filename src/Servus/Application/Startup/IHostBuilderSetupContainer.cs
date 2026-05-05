using Microsoft.Extensions.Hosting;

namespace Servus.Application.Startup;

public interface IHostBuilderSetupContainer : ISetupContainer
{
    void ConfigureHostBuilder(IHostBuilder builder);
}