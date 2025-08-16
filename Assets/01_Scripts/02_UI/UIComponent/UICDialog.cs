using Client;
using Cysharp.Threading.Tasks;
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
    private bool _endSentence = false;
    private bool _endDialog = false;
    private void Awake()
    {
        _background.onClick.AddListener(OnClose);
    }

    public async UniTask StartDialog(DialogData data = null)
    {
        _endDialog = false;
        _endSentence = false;
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

        foreach (var sentence in _dialog.Sentences)
        {
            await DisplaySentence(sentence);
        }

        _endSentence = true;
        _imgArrow.gameObject.SetActive(true);
        while (!_endDialog)
        {
            await UniTask.Yield(); // Wait until the dialog is closed
        }
    }
    private async UniTask DisplaySentence(string sentence)
    {
        sentence = sentence.SymbolColor("[", "]", _symbolColorHex);
        _dialogText.text = sentence;
        // Simulate waiting for user input or a delay
        await UniTask.Delay(2000); // 2 seconds delay for demonstration
    }
    private void OnClose()
    {
        if (_endSentence == false)
            return;
        _endDialog = true;
    }
}
