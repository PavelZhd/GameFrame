using System;
using Cysharp.Threading.Tasks;

namespace GameFrame.SceneLoader
{
    public delegate UniTask loadScene(string sceneName, Func<UniTask> extraProcess);
    public delegate UniTask beforeSceneSwitch();
    public delegate void afterSceneSwitch();
    public class SceneLoader 
    {
        private readonly beforeSceneSwitch _beforeLoad;
        private readonly afterSceneSwitch _afterScene;

        public SceneLoader(beforeSceneSwitch beforeLoad, afterSceneSwitch afterScene) {
            this._beforeLoad = beforeLoad;
            this._afterScene = afterScene;
        }
        public async UniTask LoadScene(string sceneName, Func<UniTask> extraProcess)
        {
            if (_beforeLoad != null)
                await _beforeLoad();
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName).ToUniTask();
            if (extraProcess != null)
                await extraProcess();
            if (_afterScene != null)
                _afterScene();
            return;
        }
    }
}