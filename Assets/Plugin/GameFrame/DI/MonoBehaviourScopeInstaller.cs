using UnityEngine;

namespace GameFrame.DI
{
    public abstract class MonoBehaviourScopeInstaller : MonoBehaviour, IScopeInstaller
    {
        public abstract void Install(IScope scope);
    }
}