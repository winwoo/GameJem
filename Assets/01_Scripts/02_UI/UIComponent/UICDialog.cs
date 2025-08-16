using Client;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UICDialog : MonoBehaviour
{
    [SerializeField]
    [Link]
    private TextMeshProUGUI _dialogText;
    [SerializeField]
    private DialogData _dialog;
    [SerializeField]
    private Color _symbolColor;

    private string _symbolColorHex;

    public async UniTask StartDialog()
    {
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

        gameObject.SetActive(false); // Hide dialog after displaying all sentences
    }
    private async UniTask DisplaySentence(string sentence)
    {
        sentence = sentence.SymbolColor("[", "]", _symbolColorHex);
        _dialogText.text = sentence;
        // Simulate waiting for user input or a delay
        await UniTask.Delay(2000); // 2 seconds delay for demonstration
    }
}
