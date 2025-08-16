using Client;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class UICDialog : MonoBehaviour
{
    [SerializeField]
    private DialogData _dialog;
    [SerializeField]
    private Color _symbolColor;

    public async UniTask StartDialog()
    {
        if(_dialog == null)
        {
            Debug.LogError("DialogData is not assigned.");
            return;
        }

        foreach (var sentence in _dialog.Sentences)
        {
            await DisplaySentence(sentence);
        }
    }
    private async UniTask DisplaySentence(string sentence)
    {
        sentence = sentence.SymbolColor("[", "]", _symbolColor.ToHexString());
        Debug.Log(sentence);
        // Simulate waiting for user input or a delay
        await UniTask.Delay(2000); // 2 seconds delay for demonstration
    }
}
