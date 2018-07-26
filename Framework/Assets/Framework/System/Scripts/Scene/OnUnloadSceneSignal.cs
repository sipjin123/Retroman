namespace Framework
{
    public class OnUnloadSceneSignal
    {
        /// <summary>
        /// The ID of the scene.
        /// </summary>
        public EScene Scene { get; set; }

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string SceneName { get; set; }

        //public OnUnloadSceneSignal(EScene scene) : this(scene.ToString())
        public OnUnloadSceneSignal(EScene scene)
        {
            Scene = scene;
            SceneName = scene.ToString();
        }

        public OnUnloadSceneSignal(string scene)
        {
            Scene = EScene.Invalid;
            SceneName = scene;
        }

        public OnUnloadSceneSignal(EScene scene, string sceneName)
        {
            Scene = scene;
            SceneName = sceneName;
        }
    }
}
