using Client;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICDialog : MonoBehaviour
{
    [SerializeField]
    [Link]
    private Button _background;
    [SerializeField]
    [Link]
    private TextMeshProUGUI _dialogText;
    [SerializeField]
    [Link]
    private Image _imgArrow;
    [SerializeField]
    private DialogData _dialog;
    [SerializeField]
    private Color _symbolColor;

    private string _symbolColorHex;
    private int _index = 0;
    private DateTime _lastClick;
    private void Awake()
    {
        _background.onClick.AddListener(OnClose);
    }

    public async UniTask StartDialog(DialogData data = null)
    {
        _index = 0;
        _imgArrow.gameObject.SetActive(false);
        if (data != null)
        {
            _dialog = data;
        }
        _symbolColorHex = ColorUtility.ToHtmlStringRGB(_symbolColor);
        if(_dialog == null)
        {
            Debug.LogError("DialogData is not assigned.");
            return;
        }
        gameObject.SetActive(true);

        while (_index < _dialog.Sentences.Count)
        {
            DisplaySentence(_index++);
            _imgArrow.gameObject.SetActive(false);
            await UniTask.Delay(500);
            _imgArrow.gameObject.SetActive(true);
            while (_imgArrow.gameObject.activeSelf)
            {
                await UniTask.Yield();
            }
        }
    }
    private void DisplaySentence(int index)
    {
        string sentence = _dialog.Sentences[index];
        sentence = sentence.SymbolColor("[", "]", _symbolColorHex);
        _dialogText.text = sentence;
    }
    private void OnClose()
    {
        if((DateTime.Now - _lastClick).TotalSeconds < 0.5f)
            return;

        if (_imgArrow.gameObject.activeSelf == false)
            return;

        _lastClick = DateTime.Now;
        _imgArrow.gameObject.SetActive(false);
    }
}
