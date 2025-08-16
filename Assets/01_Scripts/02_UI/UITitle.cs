using UnityEngine;
using UnityEngine.UI;

public class UITitle : UIBase
{
    [SerializeField]
    [Link]
    private Button _btnUpdate;
    [SerializeField]
    private DialogData _dialog;

    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnUpdate.onClick.AddListener(OnUpdate);
    }

    private async void OnUpdate()
    {
        await Managers.Scene.LoadSceneAsync(Define.Scene.Game);
        await CloseUI();
    }
}
