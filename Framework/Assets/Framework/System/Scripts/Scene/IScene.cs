namespace Framework
{
    public interface IScene
    {
        ISceneData SceneData { get; set; }
        T GetSceneData<T>() where T : ISceneData;

        bool IsPersistent { get; }
    }
}
