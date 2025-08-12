using UnityEngine;

public enum ResourceBackend { Resources, Addressables, AssetBundle, Hybrid }
[CreateAssetMenu(fileName = "ResourceSettings", menuName = "Config/ResourceSettings")]
public class ResourceSettings : ScriptableObject
{
    [Header("�鿣�� ����")]
    public ResourceBackend backend = ResourceBackend.Resources;

    [Header("Addressables ���� �� Resources ���� (Hybrid������ �ǹ� ����)")]
    public bool fallbackToResources = true;

    [Header("���� ĳ��(����ī��Ʈ) ���")]
    public bool enableCaching = true;
}
