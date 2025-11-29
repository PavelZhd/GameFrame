using Cysharp.Threading.Tasks;
using GameFrame.DI;
using UnityEngine;

namespace GameFrame.Unity.DI
{
    public delegate UniTask StartGame();
    public class Boostrap : MonoBehaviour
    {
        [SerializeField] private MonoBehaviourScopeInstaller[] _monoBehaviourInstallers;
        [SerializeField] private ScriptableObjectScopeInstaller[] _scriptableObjectInstallers;

        private async void Start()
        {
            var rootScope = Scope.Root();
            foreach (IScopeInstaller installer in _scriptableObjectInstallers)
            {
                installer.Install(rootScope);
            }
            foreach (IScopeInstaller installer in _monoBehaviourInstallers)
            {
                installer.Install(rootScope);
            }
            await rootScope.Resolve<StartGame>()();
            rootScope.Dispose();
            Application.Quit();
        }
    }
}