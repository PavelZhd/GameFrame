using GameFrame.DI;

namespace GameFrame.Unity.DI
{
    public abstract class MonoBehaviourScopeInjector : MonoBehaviourScopeInstaller, IScopeInjector
    {
        public abstract void Inject(IResolvingScope scope);
    }
}