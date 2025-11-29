using GameFrame.DI;
using UnityEngine;
namespace GameFrame.Unity.DI
{
    public abstract class ScriptableObjectScopeInstaller : ScriptableObject, IScopeInstaller
    {
        public abstract void Install(IRegistrableScope scope);
    }
}