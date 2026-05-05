using Servus.Threading.Tasks;

namespace Servus.Application.Startup.Gates;

public interface ISetupStartupGates
{
    void OnRegisterStartupGates(IActionRegistry<IStartupGate> registry);
}