using System;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : UIBase
{
    [SerializeField]
    [Link]
    private Button _btnOk;
    [SerializeField]
    [Link]
    private Button _btnCancel;

    private Action<bool> _onResult;

    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnOk.onClick.AddListener(OnOkClick);
        _btnCancel.onClick.AddListener(OnCancelClick);
        _onResult = ctx as Action<bool>;
    }
    
    private async void OnOkClick()
    {
        _onResult?.Invoke(true);
        _onResult = null;
        await CloseUI();
    }

    private async void OnCancelClick()
    {
        _onResult?.Invoke(false);
        _onResult = null;
        await CloseUI();
    }

}
