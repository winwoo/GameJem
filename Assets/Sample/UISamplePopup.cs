using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[UIPath("Sample")]
public class UISamplePopup : UIBase
{
    [SerializeField]
    [Link]
    private Button _closeButton;

    public override void OnCreate(object ctx)
    {
        _closeButton.onClick.AddListener(OnClose);
    }

    private async void OnClose()
    {
        await CloseUI();
    }
}
