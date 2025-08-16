using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : UIBase
{
    [SerializeField]
    [Link]
    private UICDialog _dialog;

    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
    }

    public override async UniTask ShowAsync(object args = null)
    {
        await base.ShowAsync(args);
        await _dialog.StartDialog();
        await Managers.Scene.LoadSceneAsync(Define.Scene.Game);
        await CloseUI();
    }

    private async void OnUpdate()
    {
        await Managers.Scene.LoadSceneAsync(Define.Scene.Game);
        await CloseUI();
    }
}
