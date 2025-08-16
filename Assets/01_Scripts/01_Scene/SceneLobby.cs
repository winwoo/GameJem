using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneLobby : SceneBase
{
    protected async override UniTask Init()
    {
        await Managers.UI.Open<UISteam>();
    }

    public override UniTask Dispose()
    {
        return UniTask.CompletedTask;
    }
}
