using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using Logger = Client.Logger;
using System.Linq;
public class ResourceManager : BaseManager
{
    public ResourceBackend Backend => _settings.backend;

    private ResourceSettings _settings;
    private IResourceProvider _res;
    private IResourceProvider _addr;
    private IEnumerable<IResourceProvider> _providers;
    private bool _fallbackResources;

    private readonly Dictionary<CacheKey, CacheEntry> _assetCache = new(); // key -> entry
    public async override UniTask Init()
    {
        _settings = Resources.Load<ResourceSettings>("99_Config/ResourceSettings");
        if (_settings == null)
        {
            Logger.LogError("ResourceManager", "ResourceSettings not found! Please create it in Resources folder.");
            return;
        }

        _res = new ResourcesProvider();
        _addr = new AddressablesProvider();

        if(Backend == ResourceBackend.Addressables || Backend == ResourceBackend.Hybrid)
        {
            await _addr.InitializeAsync();
        }
        _providers = ProviderOrder();
        _fallbackResources = _settings.fallbackToResources && _providers.All(p => p != _res);
    }

    public async override UniTask Dispose()
    {
        ClearCache();
        await UniTask.CompletedTask;
    }

    #region Load
    /// <summary>
    /// 비동기 에셋 로드
    /// </summary>
    /// <param name="key">
    /// Resources : Resources 하위 경로 (예: "Textures/myTexture")
    /// Addressables : Addressable 키 (예: "myTextureKey")
    /// </param>
    /// <returns>에셋</returns>
    public async UniTask<T> LoadAsync<T>(string key) where T : UnityEngine.Object
    {
        if (TryGetFromCache(key, out T cached))
            return cached;

        foreach (var provider in _providers)
        {
            var (asset, handle) = await provider.LoadAssetAsync<T>(key);
            if (asset != null)
                return Cache(key, asset, provider, handle);
        }

        if (_fallbackResources)
        {
            var (asset, handle) = await _res.LoadAssetAsync<T>(key);
            if (asset != null)
                return Cache(key, asset, _res, handle);
        }

        return null;
    }

    public T LoadSync<T>(string key) where T : UnityEngine.Object
    {
        if (Backend == ResourceBackend.Addressables)
            throw new InvalidOperationException("Addressables 모드에서는 동기 로드를 지원하지 않습니다.");

        if (TryGetFromCache(key, out T cached))
            return cached;

        // Resources 우선
        var asset = _res.LoadAssetSync<T>(key);
        return Cache(key, asset, _res, handle: null);
    }
    #endregion

    #region Release
    /// <summary>
    /// 캐시 해제 및 에셋 반환
    /// </summary>
    /// <param name="key">
    /// Resources : Resources 하위 경로 (예: "Textures/myTexture")
    /// Addressables : Addressable 키 (예: "myTextureKey")
    /// </param>
    public void Release<T>(string key)
    {
        var ck = CK<T>(key);
        if (!_assetCache.TryGetValue(ck, out var e)) return;
        e.RefCount--;
        if (e.RefCount > 0) return;

        e.Provider.ReleaseAsset(e.Handle, e.Asset);
        _assetCache.Remove(ck);
    }

    public void ClearCache()
    {
        // 에셋 캐시 해제
        foreach (var kv in _assetCache.ToArray())
        {
            var e = kv.Value;
            e.Provider.ReleaseAsset(e.Handle, e.Asset);
        }
        _assetCache.Clear();
    }
    #endregion

    #region Internal Utils
    private IEnumerable<IResourceProvider> ProviderOrder()
    {
        return Backend switch
        {
            ResourceBackend.Resources => new[] { _res },
            ResourceBackend.Addressables => new[] { _addr },
            ResourceBackend.Hybrid => new[] { _addr, _res },
            _ => new[] { _res }
        };
    }

    private bool TryGetFromCache<T>(string key, out T asset) where T : UnityEngine.Object
    {
        asset = null;
        if (!_settings.enableCaching) 
            return false;

        if (_assetCache.TryGetValue(CK<T>(key), out var e) && e.Asset is T t)
        {
            e.RefCount++;
            asset = t;
            return true;
        }
        return false;
    }

    private T Cache<T>(string key, T asset, IResourceProvider provider, object handle) where T : UnityEngine.Object
    {
        if (asset == null) 
            return null;

        if (!_settings.enableCaching)
            return asset;

        var ck = CK<T>(key);
        if (_assetCache.TryGetValue(ck, out var e))
        {
            e.Asset = asset;
            e.Provider = provider;
            e.Handle = handle;
            e.RefCount++;
            return asset;
        }

        _assetCache[ck] = new CacheEntry
        {
            Asset = asset,
            Provider = provider,
            Handle = handle,
            RefCount = 1
        };
        return asset;
    }

    sealed class CacheEntry
    {
        public UnityEngine.Object Asset;
        public IResourceProvider Provider;
        public object Handle;
        public int RefCount;
    }

    readonly struct CacheKey : IEquatable<CacheKey>
    {
        public readonly string Key;
        public readonly Type Type;
        public CacheKey(string key, Type type) { Key = key; Type = type; }
        public bool Equals(CacheKey other) => Key == other.Key && Type == other.Type;
        public override bool Equals(object obj) => obj is CacheKey o && Equals(o);
        public override int GetHashCode() => HashCode.Combine(Key, Type);
    }

    private CacheKey CK<T>(string key) => new CacheKey(key, typeof(T));
    #endregion
}
