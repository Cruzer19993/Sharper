using MonoGAME.Systems.Backend;
using MonoGAME.Structures;
using System;

namespace MonoGAME.Systems.Backend.Management
{
    public class SceneManager
    {
        public static EventHandler<SceneEventArgs> OnSceneLoad;
        public static Scene CurrentScene
        {
            get { return currentScene; }
            private set { currentScene = value; }
        }
        private static Scene currentScene;
        public Scene GetCurrentScene()
        {
            return currentScene;
        }
        public void LoadScene(Scene scene)
        {
            CurrentScene = scene;
            scene.OnSceneLoad(this);
        }

        public void DoneLoadingScene()
        {
            if(OnSceneLoad != null)
                OnSceneLoad.Invoke(this, new SceneEventArgs(currentScene));
        }
    }
    public class SceneEventArgs : EventArgs
    {
        public SceneEventArgs(Scene scene)
        {
            argScene = scene;
        }
        public Scene argScene;
    }
}
