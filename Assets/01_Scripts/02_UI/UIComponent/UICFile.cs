using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICFile : MonoBehaviour
{
    [SerializeField]
    [Link]
    private Image _imgSelect;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _textName;
    [SerializeField]
    [Link]
    private Button _btnIcon;

    public bool IsSelected => _imgSelect.gameObject.activeSelf;
    public int Index { get; private set; } = -1;
    private Action<UICFile> _onClickIcon;
    private void Awake()
    {
        _textName.text = string.Empty;
    }

    public void AddEvent(Action<UICFile> onClickIcon, int index)
    {
        _onClickIcon = onClickIcon;
        Index = index;
        _btnIcon.onClick.AddListener(OnClickIcon);
        SetSelct(false);
    }

    public void SetSelct(bool active)
    {
        _imgSelect.gameObject.SetActive(active);
    }

    public void SetText(string text)
    {
        _textName.text = $"{text}.txt";
    }

    private void OnClickIcon()
    {
        _onClickIcon?.Invoke(this);
    }
}
