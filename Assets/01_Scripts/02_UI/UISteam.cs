using System.Linq;
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
        var datas = Managers.Instance.InitBugSetting.InitBugData;
        // datas 의 모든 IsBug가 false인지
        bool allBugsFixed = datas.All(data => !data.IsBug);
        Managers.Instance.PlayCount++;
        if (Managers.Instance.PlayCount < 3 && allBugsFixed == false)
        {
            await Managers.UI.Open<UIForlder>();
        }
        else
        {
            await Managers.UI.Open<UIEnding>();
        }
        await CloseUI();
    }
}