using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class SoundManager : BaseManager
{
    private readonly string _basePath = "02_Audio";
    private readonly int _poolCount = 12;

    public float BgmVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;

    private Transform _root;
    private AudioHost _host;
    private AudioSource _bgm;
    private readonly Queue<AudioSource> _sfxFree = new();
    private readonly HashSet<AudioSource> _sfxInUse = new();


    public async override UniTask Init()
    {
        if (_root != null)
            return;

        GameObject root = GameObject.Find("@Sound_Root");
        if (root == null)
            _root = new GameObject { name = "@Sound_Root" }.transform;
        else
            _root = root.transform;

        if (Application.isPlaying)
        {
            Object.DontDestroyOnLoad(_root);
            _host = _root.GetOrAddComponent<AudioHost>();
            _bgm = _root.GetOrAddComponent<AudioSource>();
            _bgm.playOnAwake = false;
            _bgm.loop = true;
            _bgm.spatialBlend = 0f;
            _bgm.volume = BgmVolume;

            // SFX Pool
            for (int i = 0; i < _poolCount; i++)
                _sfxFree.Enqueue(CreateSfxSource());
        }
        await UniTask.CompletedTask;
    }

    public async override UniTask Dispose()
    {
        DestroyAll(_sfxInUse);
        DestroyAll(_sfxFree);
        _sfxInUse.Clear();
        _sfxFree.Clear();

        if (_bgm) _bgm.Stop();
        _bgm = null;

        if (_root) Object.Destroy(_root.gameObject);
        _root = null;
        _host = null;
        await UniTask.CompletedTask;
    }

    private void DestroyAll(IEnumerable<AudioSource> sfx)
    {
        foreach(var s in sfx)
        {
            if (s)
            {
                Object.Destroy(s.gameObject);
            }
        }
    }

    public async UniTask PlayBGM(string key, bool loop = true, float fadeTime = 0f)
    {
        string path = $"{_basePath}/BGM/{key}";
        var clip = await Managers.Resource.LoadAsync<AudioClip>(path);
        if (clip == null) return;

        _bgm.loop = loop;

        if (fadeTime > 0f && _bgm.isPlaying)
            await FadeVolume(_bgm, _bgm.volume, 0f, fadeTime);

        _bgm.clip = clip;
        _bgm.volume = BgmVolume;
        _bgm.Play();
    }

    public async UniTask StopBGM(float fadeTime = 0f)
    {
        if (!_bgm.isPlaying) return;

        if (fadeTime > 0f)
            await FadeVolume(_bgm, _bgm.volume, 0f, fadeTime);

        _bgm.Stop();
        _bgm.clip = null;
    }

    public void SetBgmVolume(float v)
    {
        BgmVolume = Mathf.Clamp01(v);
        if (_bgm) _bgm.volume = BgmVolume;
    }

    // ====== SFX ======
    public async UniTask PlaySFX(string key, float pitch = 1f, float volume = 0f)
    {
        string path = $"{_basePath}/SFX/{key}";
        var clip = await Managers.Resource.LoadAsync<AudioClip>(path);
        if (clip == null) return;

        var src = GetSfx();
        src.clip = clip;
        src.pitch = pitch;
        src.volume = volume == 0 ? SfxVolume : volume;
        src.spatialBlend = 0f;
        src.transform.position = Vector3.zero;
        src.loop = false;
        src.Play();

        ReturnAfter(src, clip.length).Forget();
    }

    // 3D SFX
    public async UniTask PlaySFXAt(string key, Vector3 worldPos, float spatialBlend = 1f, float maxDistance = 25f, float pitch = 1f)
    {
        string path = $"{_basePath}/SFX/{key}";
        var clip = await Managers.Resource.LoadAsync<AudioClip>(path);
        if (clip == null) return;

        var src = GetSfx();
        src.transform.position = worldPos;
        src.spatialBlend = Mathf.Clamp01(spatialBlend);
        src.maxDistance = maxDistance;
        src.pitch = pitch;
        src.volume = SfxVolume;
        src.loop = false;
        src.PlayOneShot(clip);

        ReturnAfter(src, clip.length).Forget();
    }

    public void SetSfxVolume(float v)
    {
        SfxVolume = Mathf.Clamp01(v);
        SetSfxVolume(_sfxInUse);
        SetSfxVolume(_sfxFree);
    }

    private void SetSfxVolume(IEnumerable<AudioSource> sources)
    {
        foreach (var s in sources)
        {
            if (s) s.volume = SfxVolume;
        }
    }

    // ====== Internals ======
    private AudioSource CreateSfxSource()
    {
        var go = new GameObject("SFX_AudioSource");
        go.transform.SetParent(_root.transform, false);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.volume = SfxVolume;
        src.spatialBlend = 0f;
        return src;
    }

    private AudioSource GetSfx()
    {
        var s = _sfxFree.Count > 0 ? _sfxFree.Dequeue() : CreateSfxSource();
        _sfxInUse.Add(s);
        return s;
    }

    private void ReturnSfx(AudioSource s)
    {
        if (s == null) return;
        s.Stop();
        s.clip = null;
        s.pitch = 1f;
        s.transform.SetParent(_root.transform, false);
        s.spatialBlend = 0f;
        _sfxInUse.Remove(s);
        _sfxFree.Enqueue(s);
    }

    private async UniTaskVoid ReturnAfter(AudioSource s, float clipLen)
    {
        if (s == null) 
            return;

        if (_root == null)
        {
            if (s) Object.Destroy(s.gameObject);
            _sfxInUse.Remove(s);
            return;
        }

        var ct = _host.Token; // 파괴/씬전환 시 자동 취소
        try
        {
            // length + 한 프레임 여유
            var dur = clipLen / Mathf.Max(0.01f, s.pitch);
            await UniTask.Delay(TimeSpan.FromSeconds(dur), cancellationToken: ct);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        catch (OperationCanceledException) { /* 씬 전환 등 */ }
        ReturnSfx(s);
    }

    private async UniTask FadeVolume(AudioSource src, float from, float to, float dur)
    {
        var ct = _host.Token;
        if (dur <= 0f) { src.volume = to; return; }

        float t = 0f;
        while (t < dur && src != null)
        {
            t += Time.unscaledDeltaTime;   // UI/일시정지 영향 피하려면 unscaled
            src.volume = Mathf.Lerp(from, to, t / dur);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        if (src != null) src.volume = to;
    }
}