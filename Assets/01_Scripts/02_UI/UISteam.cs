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
        if (Managers.BattleDesign.PlayerDesignForlder.Count == 0 && Managers.BattleDesign.BossDesignForlder.Count == 0)
        {
            await Managers.UI.Open<UIEnding>();
        }
        else
        {
            await Managers.UI.Open<UIForlder>();
        }
        await CloseUI();
    }
}