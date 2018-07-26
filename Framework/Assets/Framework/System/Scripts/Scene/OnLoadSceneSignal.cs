namespace Framework
{
    /// <summary>
    /// The given scene has started loading.
    /// </summary>
    public class OnLoadSceneSignal
    {
        /// <summary>
        /// The ID of the scene.
        /// </summary>
        public EScene Scene { get; set; }

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string SceneName { get; set; }

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public bool IsRootScene { get; set; }

        //public OnLoadSceneSignal(EScene scene) : this(scene.ToString())
        public OnLoadSceneSignal(EScene scene, bool isRoot = false)
        {
            Scene = scene;
            SceneName = scene.ToString();
            IsRootScene = isRoot;
        }

        public OnLoadSceneSignal(string scene, bool isRoot = false)
        {
            Scene = EScene.Invalid;
            SceneName = scene;
        }

        public OnLoadSceneSignal(EScene scene, string sceneName, bool isRoot = false)
        {
            Scene = scene;
            SceneName = sceneName;
        }
    }
}
