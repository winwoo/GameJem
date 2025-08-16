using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICFile : MonoBehaviour
{
    [SerializeField]
    [Link]
    protected Image _imgSelect;
    [SerializeField]
    [Link]
    protected TextMeshProUGUI _textName;
    [SerializeField]
    [Link]
    protected Button _btnIcon;
    [SerializeField]
    [Link]
    protected Transform _iconCheck;

    private Sprite _codeImg;

    private DateTime _selectTime;
    public bool IsBug { get; private set; }
    private Action<UICFile> _onClickFile;
    private void Awake()
    {
        _textName.text = string.Empty;
    }

    public void InitFile(Sprite codeImg, string name, bool isBug, Action<UICFile> onClickFile)
    {
        _codeImg = codeImg;
        _textName.text = $"{name}.txt";
        _btnIcon.onClick.AddListener(OnClickFile);
        _imgSelect.gameObject.SetActive(false); // 선택 이미지 비활성화
        IsBug = isBug;
        _onClickFile = onClickFile;
    }

    public void SetSelct(bool active)
    {
        _imgSelect.gameObject.SetActive(active);
        _iconCheck.gameObject.SetActive(active);
    }

    private async void OnClickFile()
    {
        if (_selectTime != default && (DateTime.Now - _selectTime).TotalSeconds < 0.5f)
        {
            UICode uiCode = await Managers.UI.Open<UICode>();
            uiCode.SetCode(_codeImg);
            return;
        }
        _selectTime = DateTime.Now;
        _onClickFile?.Invoke(this);
        SetSelct(true);
    }
}
