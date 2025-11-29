namespace GameFrame.DI
{    
    public interface IScopeInstaller 
    {
        void Install(IRegistrableScope scope);
    }

    public interface IScopeInjector
    {
        void Inject(IResolvingScope scope);
    }
}