using System.Linq;
using GameFrame.DI;
using GameFrame.Unity.DI;
using UnityEngine;

namespace GameFrame.Unity.SceneLoader
{
    public class SceneContextInstaller : MonoBehaviour
    {
        [SerializeField] private MonoBehaviourScopeInstaller[] _monoBehaviourScopeInstallers;
        [SerializeField] private ScriptableObjectScopeInstaller[] _scriptableObjectScopeInstallers;

        public void InstallScope(IRegistrableScope registrableScope)
        {
            foreach (IScopeInstaller installer in _scriptableObjectScopeInstallers)
            {
                installer.Install(registrableScope);
            }
            foreach (IScopeInstaller installer in _monoBehaviourScopeInstallers)
            {
                installer.Install(registrableScope);
            }
        }
        public void InjectScope(IResolvingScope resolvingScope)
        {
            foreach (IScopeInjector injector in _scriptableObjectScopeInstallers.Where(x => x is IScopeInjector).Cast<IScopeInjector>())
            {
                injector.Inject(resolvingScope);
            }
            foreach (IScopeInjector injector in _monoBehaviourScopeInstallers.Where(x => x is IScopeInjector).Cast<IScopeInjector>())
            {
                injector.Inject(resolvingScope);
            }
        }
    }
}