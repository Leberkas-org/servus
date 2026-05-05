using Microsoft.Extensions.Hosting;

namespace Servus.Application.Startup;

public interface IHostApplicationBuilderSetupContainer : ISetupContainer
{
    void ConfigureHostApplicationBuilder(IHostApplicationBuilder builder);
}