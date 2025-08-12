using Cysharp.Threading.Tasks;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class AddressablesProvider : IResourceProvider
{
    bool _inited;

    public async UniTask InitializeAsync()
    {
        if (_inited) return;
        var h = Addressables.InitializeAsync();
        await h.Task;
        _inited = true;
    }

    public async UniTask<(T asset, object handle)> LoadAssetAsync<T>(string key) where T : Object
    {
        var h = Addressables.LoadAssetAsync<T>(key);
        await h.Task;
        if (h.Status == AsyncOperationStatus.Succeeded)
            return (h.Result, h);

        return (null, null);
    }

    public T LoadAssetSync<T>(string key) where T : Object
        => throw new System.InvalidOperationException("Addressables는 동기 로드를 지원하지 않습니다.");

    public void ReleaseAsset(object handle, Object asset)
    {
        if (handle is AsyncOperationHandle h && h.IsValid())
        {
            Addressables.Release(h);
        }
    }
}
