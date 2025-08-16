using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneTitle : SceneBase
{
    protected async override UniTask Init()
    {
        if (!Managers.Instance.IsIntro)
        {
            Managers.Instance.IsIntro = true; // 인트로 여부 설정
            await Managers.UI.Open<UITitle>();
        }
        //await Managers.UI.Open<UIForlder>();
        //await Managers.UI.Open<UISystem>();
    }

    public override UniTask Dispose()
    {
        return UniTask.CompletedTask;
    }
}
