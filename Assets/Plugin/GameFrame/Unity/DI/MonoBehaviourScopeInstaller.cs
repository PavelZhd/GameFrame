using GameFrame.DI;
using UnityEngine;

namespace GameFrame.Unity.DI
{
    public abstract class MonoBehaviourScopeInstaller : MonoBehaviour, IScopeInstaller
    {
        public abstract void Install(IRegistrableScope scope);
    }
}