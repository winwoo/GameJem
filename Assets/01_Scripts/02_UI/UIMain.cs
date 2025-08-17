using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : UIBase
{
    [SerializeField]
    [Link]
    private Image _imgBackground;
    [SerializeField]
    [Link]
    private Button _btnGmail;

    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnGmail.onClick.AddListener(OnClickGmail);
    }

    public override async UniTask ShowAsync(object args = null)
    {
        await base.ShowAsync(args);
        await Managers.Sound.PlayBGM("Kerning Square (Title-StoryBegin)");
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * 5f) * 10f; // 애니메이션 효과
        _btnGmail.transform.localPosition = new Vector3(_btnGmail.transform.localPosition.x, y, _btnGmail.transform.localPosition.z);
    }

    private async void OnClickGmail()
    {
        await Managers.UI.Open<UITitle>();
        await CloseUI();
    }
}
