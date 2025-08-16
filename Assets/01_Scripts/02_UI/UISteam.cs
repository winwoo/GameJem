using UnityEngine;
using UnityEngine.UI;

public class UISteam : UIBase
{
    [SerializeField]
    [Link]
    private Button _btnClose;

    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnClose.onClick.AddListener(OnClickClose);
    }

    private async void OnClickClose()
    {
        await Managers.UI.Open<UIForlder>();
        await CloseUI();
    }
}