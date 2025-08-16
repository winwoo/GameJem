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

    private Sprite _codeImg;

    private DateTime _selectTime;
    private void Awake()
    {
        _textName.text = string.Empty;
    }

    public void InitFile(Sprite codeImg, string name)
    {
        _codeImg = codeImg;
        _textName.text = $"{name}.txt";
        _btnIcon.onClick.AddListener(OnClickFile);
        _imgSelect.gameObject.SetActive(false); // 선택 이미지 비활성화
    }

    public void SetSelct(bool active)
    {
        _imgSelect.gameObject.SetActive(active);
    }

    private void OnClickFile()
    {
        if (_selectTime != default && (DateTime.Now - _selectTime).TotalSeconds < 0.5f)
        {
            // 더블 클릭 처리
            Debug.Log("img open");
            SetSelct(false);
            return;
        }
        _selectTime = DateTime.Now;
        SetSelct(true);
    }
}
