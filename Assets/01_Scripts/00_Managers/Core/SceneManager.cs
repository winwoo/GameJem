using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SceneManager : BaseManager
{
    public SceneBase CurrentScene { get { return GameObject.FindAnyObjectByType<SceneBase>(); } }

    public async override UniTask Init()
    {
        await UniTask.CompletedTask;
    }

    public async override UniTask Dispose()
    {
        if (CurrentScene != null)
        {
            await CurrentScene.Dispose();
        }
        await UniTask.CompletedTask;
    }

    public void LoadScene(Define.Scene type, Action onLoaded)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GetSceneName(type));
    }

    public async UniTask LoadSceneAsync(Define.Scene type)
    {
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName(type));
    }

    private string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return $"0{(int)type}_{name}";
    }
}
