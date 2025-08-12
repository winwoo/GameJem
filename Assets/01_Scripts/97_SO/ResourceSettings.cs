using UnityEngine;

public enum ResourceBackend { Resources, Addressables, AssetBundle, Hybrid }
[CreateAssetMenu(fileName = "ResourceSettings", menuName = "Config/ResourceSettings")]
public class ResourceSettings : ScriptableObject
{
    [Header("백엔드 선택")]
    public ResourceBackend backend = ResourceBackend.Resources;

    [Header("Addressables 실패 시 Resources 폴백 (Hybrid에서도 의미 있음)")]
    public bool fallbackToResources = true;

    [Header("에셋 캐시(참조카운트) 사용")]
    public bool enableCaching = true;
}
