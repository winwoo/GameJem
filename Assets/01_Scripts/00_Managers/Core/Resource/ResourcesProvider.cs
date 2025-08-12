using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ResourcesProvider : IResourceProvider
{
    public UniTask InitializeAsync() => UniTask.CompletedTask;

    public async UniTask<(T asset, object handle)> LoadAssetAsync<T>(string key) where T : Object
    {
        var req = Resources.LoadAsync<T>(key);
        await req;
        return ((T)req.asset, null);
    }

    public T LoadAssetSync<T>(string key) where T : Object
        => Resources.Load<T>(key);

    public void ReleaseAsset(object handle, Object asset)
    {
        if (asset != null && asset is not GameObject)
            Resources.UnloadAsset(asset);
        // GameObject 프리팹 에셋은 UnloadAsset 비권장
    }
}