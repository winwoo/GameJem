using UnityEngine;
using UnityEngine.UI;

public class UICode : UIBase
{
    [SerializeField]
    [Link]
    private Image _imgCode;
    [SerializeField]
    [Link]
    private Button _btnClose;
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnClose.onClick.AddListener(async () =>
        {
            await CloseUI();
        });
    }

    public void SetCode(Sprite sp)
    {
        _imgCode.sprite = sp;
    }
}
