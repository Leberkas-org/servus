namespace Servus.Application.Startup;

public abstract class ApplicationSetupContainer : ISetupContainer
{
    internal void InjectApp(ServusApplication app) => SetupApplication(app);

    protected abstract void SetupApplication(ServusApplication app);
}

public abstract class ApplicationSetupContainer<THost> : ApplicationSetupContainer
    where THost : ServusApplication
{
    protected sealed override void SetupApplication(ServusApplication app)
    {
        SetupApplication((THost)app);
    }

    protected abstract void SetupApplication(THost app);
}
