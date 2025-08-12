using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IResourceProvider
{
    UniTask InitializeAsync();

    UniTask<(T asset, object handle)> LoadAssetAsync<T>(string key) where T : Object;
    T LoadAssetSync<T>(string key) where T : Object; // SupportsSync=false¸é throw

    void ReleaseAsset(object handle, Object asset);
}