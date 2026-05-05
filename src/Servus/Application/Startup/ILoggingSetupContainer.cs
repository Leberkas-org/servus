using Microsoft.Extensions.Logging;

namespace Servus.Application.Startup;

public interface ILoggingSetupContainer : ISetupContainer
{
    void SetupLogging(ILoggingBuilder builder);
}