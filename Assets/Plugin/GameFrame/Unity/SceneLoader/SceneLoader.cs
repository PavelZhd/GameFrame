using System;
using Cysharp.Threading.Tasks;
using GameFrame.DI;
using UnityEngine;

namespace GameFrame.Unity.SceneLoader
{
    public delegate UniTask loadScene(string sceneName, IScope sceneScopeParent, Func<UniTask> extraProcess);
    public delegate UniTask beforeSceneSwitch();
    public delegate void afterSceneSwitch();
    public class SceneLoader 
    {
        private readonly beforeSceneSwitch _beforeLoad;
        private readonly afterSceneSwitch _afterScene;
        private IScope _sceneScope;

        public SceneLoader(beforeSceneSwitch beforeLoad, afterSceneSwitch afterScene) {
            this._beforeLoad = beforeLoad;
            this._afterScene = afterScene;
        }
        public async UniTask LoadScene(string sceneName, IScope sceneScopeParent, Func<UniTask> extraProcess)
        {
            if (_beforeLoad != null)
                await _beforeLoad();
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName).ToUniTask();
            if (_sceneScope != null) {
                _sceneScope.Dispose();
                _sceneScope = null;
            }
            if (sceneScopeParent != null) { 
                _sceneScope = sceneScopeParent.CreateChild();
                var contextInstallers = UnityEngine.Object.FindObjectsByType<SceneContextInstaller>(FindObjectsSortMode.None);
                foreach (var installer in contextInstallers)
                {
                    installer.InstallScope(_sceneScope);
                }
                foreach (var installer in contextInstallers)
                {
                    installer.InjectScope(_sceneScope);
                }
            }
            if (extraProcess != null)
                await extraProcess();
            
            if (_afterScene != null)
                _afterScene();
            return;
        }
    }
}