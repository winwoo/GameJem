using Cysharp.Threading.Tasks;
using DesignData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UIPath("Sample")]
public class UISample : UIBase
{
    [SerializeField][Link]
    private Button _dataLoadButton;
    [SerializeField][Link]
    private Button _openPopupButton;
    [SerializeField][Link]
    private Button _onOffBgmButton;
    [SerializeField][Link]
    private Button _playSfxButton;
    [SerializeField][Link]
    private TextMeshProUGUI _resultText;
    [SerializeField][Link]
    private TMP_InputField _inputDataId;
    [SerializeField][Link]
    private Slider _bgmVolume;

    bool _isBgmOn = false;
    public override void OnCreate(object ctx)
    {
        base.OnCreate(ctx);
        _dataLoadButton.onClick.AddListener(OnDataLoad);
        _openPopupButton.onClick.AddListener(OnOpenPopup);
        _onOffBgmButton.onClick.AddListener(OnOnOffBgm);
        _playSfxButton.onClick.AddListener(OnPlaySfx);
        _bgmVolume.value = Managers.Sound.BgmVolume; // 초기 볼륨 설정
        _bgmVolume.onValueChanged.AddListener(OnBgmVolumeChanged);
    }


    private void OnDataLoad()
    {
        string value = _inputDataId.text;
        if (string.IsNullOrEmpty(value))
        {
            _resultText.text = "Please enter a valid ID.";
            return;
        }

        if (!int.TryParse(value, out int dataId))
        {
            _resultText.text = "Invalid ID format.";
            return;
        }
        var sample = Managers.Data.GetData<SampleTable>(dataId);
        if (sample == null)
        {
            _resultText.text = $"No data found for ID: {dataId}";
            return;
        }
        _resultText.text = sample.ToString();
    }

    private async void OnOpenPopup()
    {
        await Managers.UI.Open<UISamplePopup>();
    }

    private async void OnOnOffBgm()
    {
        if (_isBgmOn)
        {
            await Managers.Sound.StopBGM(1f);
        }
        else
        {
            await Managers.Sound.PlayBGM("Sample");
        }
        _isBgmOn = !_isBgmOn;
    }

    private async void OnPlaySfx()
    {
        await Managers.Sound.PlaySFX("Sample");
    }

    private void OnBgmVolumeChanged(float value)
    {
        Managers.Sound.SetBgmVolume(value);
    }
}
