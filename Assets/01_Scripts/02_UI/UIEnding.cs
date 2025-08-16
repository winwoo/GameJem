using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIEnding : UIBase
{
    [SerializeField]
    [Link]
    private UICDialog _dialog;
    [SerializeField]
    [Link]
    private Image _backgroundBad;
    [SerializeField]
    [Link]
    private Image _backgroundHappy;

    [SerializeField]
    private DialogData _badEndingDialog;
    [SerializeField]
    private DialogData _happyEndingDialog;

    private bool _isBad;
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        bool isBad = (bool)ctx;
        _isBad = isBad;
        _backgroundBad.gameObject.SetActive(isBad);
        _backgroundHappy.gameObject.SetActive(!isBad);
    }

    public override async UniTask ShowAsync(object args = null)
    {
        await base.ShowAsync(args);
        _dialog.gameObject.SetActive(false);
        if (_isBad)
        {
            await _dialog.StartDialog(_badEndingDialog);
        }
        else
        {
            await _dialog.StartDialog(_happyEndingDialog);
        }
        Managers.Instance.IsIntro = false;
        await Managers.Scene.LoadSceneAsync(Define.Scene.Title);
        await CloseUI();
    }
}
