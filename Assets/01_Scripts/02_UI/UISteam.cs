using Client;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISteam : UIBase
{
    [SerializeField]
    [Link]
    private Button _btnClose;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _textBefore;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _textAfter;
    [SerializeField]
    [Link]
    private RectTransform _texts;

    [SerializeField]
    private Color _goodColor;
    [SerializeField]
    private Color _neutralColor;
    [SerializeField]
    private Color _badColor;

    private string[] _scoreText = new string[]
    {
        "매우 부정적",
        "대체로 부정적",
        "복합적",
        "대체로 긍정적",
        "매우 긍정적",
        "압도적 긍정적"
    };
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _btnClose.onClick.AddListener(OnClickClose);

        // value가 false인 count
        int beforeCount = Managers.Instance.OriginBugs.Count(data => !data.Value);
        int afterCount = Managers.Instance.InitBugSetting.Count(data => !data.IsBug);

        SetScoreText(beforeCount, _textBefore);
        SetScoreText(afterCount, _textAfter);
    }

    public override async UniTask ShowAsync(object args = null)
    {
        await base.ShowAsync(args);
        while(_texts.localPosition.y < 50)
        {
            await UniTask.Yield();
            _texts.localPosition += new Vector3(0, 10 * Time.deltaTime, 0);

        }
    }

    private void SetScoreText(int count, TextMeshProUGUI label)
    {
        string text = _scoreText[count];
        string hex = "";
        if (count < 2)
            hex = ColorUtility.ToHtmlStringRGB(_badColor);
        else if (count < 3)
            hex = ColorUtility.ToHtmlStringRGB(_neutralColor);
        else
            hex = ColorUtility.ToHtmlStringRGB(_goodColor);

        label.text = $"<color=#{hex}>{text}</color>";
    }

    private async void OnClickClose()
    {
        var datas = Managers.Instance.InitBugSetting;
        // datas 의 모든 IsBug가 false인지
        bool allBugsFixed = datas.All(data => !data.IsBug);
        if (Managers.Instance.PlayCount < 3 && allBugsFixed == false)
        {
            await Managers.UI.Open<UIForlder>();
        }
        else
        {
            await Managers.UI.Open<UIEnding>();
        }
        Managers.Instance.PlayCount++;
        await CloseUI();
    }
}