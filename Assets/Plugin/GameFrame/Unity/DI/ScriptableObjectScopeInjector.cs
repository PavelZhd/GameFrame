using GameFrame.DI;

namespace GameFrame.Unity.DI
{
    public abstract class ScriptableObjectScopeInjector : ScriptableObjectScopeInstaller, IScopeInjector
    {
        public abstract void Inject(IResolvingScope scope);
    }
}