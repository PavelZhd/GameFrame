using UnityEngine;
namespace GameFrame.DI
{
    public abstract class ScriptableObjectScopeInstaller : ScriptableObject, IScopeInstaller
    {
        public abstract void Install(IScope scope);
    }
}