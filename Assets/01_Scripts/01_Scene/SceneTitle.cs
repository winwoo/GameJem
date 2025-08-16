using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneTitle : SceneBase
{
    protected async override UniTask Init()
    {
        //await Managers.UI.Open<UITitle>();
        await Managers.UI.Open<UIForlder>();
    }

    public override UniTask Dispose()
    {
        return UniTask.CompletedTask;
    }
}
