using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : UIBase
{
    [SerializeField]
    [Link]
    private UICDialog _dialog;

    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
    }

    public override async UniTask ShowAsync(object args = null)
    {
        await base.ShowAsync(args);
        await Managers.Sound.PlayBGM("Kerning Square (Title-StoryBegin)");
        await UniTask.Delay(3000); // Delay to allow BGM to start playing
        Managers.Instance.IsIntro = true;
        await _dialog.StartDialog();
        await Managers.Scene.LoadSceneAsync(Define.Scene.Game);
        await CloseUI();
    }
}
