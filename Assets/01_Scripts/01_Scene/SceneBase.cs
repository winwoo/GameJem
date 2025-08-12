using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SceneBase : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    void Awake()
    {
        Init().Forget();
    }

    protected abstract UniTask Init();
    public abstract UniTask Dispose();
}
