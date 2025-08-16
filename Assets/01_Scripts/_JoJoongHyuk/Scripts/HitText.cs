using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HitText : MonoBehaviour
{
    [SerializeField] private string _positiveColorHex = "#00FF00"; // Green
    [SerializeField] private string _negativeColorHex = "#FF0000"; // Red
    [SerializeField] private TextMeshProUGUI _text;

    private CancellationTokenSource _cancellationTokenSource;

    public void ShowHitText(bool isPositive)
    {
    string colorHex = isPositive ? _positiveColorHex : _negativeColorHex;
    _text.text = isPositive ? $"<color={colorHex}>+</color>" : $"<color={colorHex}>-</color>";
    _cancellationTokenSource = new CancellationTokenSource();
    AnimateAndDestroyAsync(_cancellationTokenSource.Token).Forget();
    }

    private async UniTask AnimateAndDestroyAsync(CancellationToken token)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 50f, 0);
        Color startColor = _text.color;
        while (elapsed < duration)
        {
            if (token.IsCancellationRequested)
                return;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            _text.color = c;
            elapsed += Time.deltaTime;
            await UniTask.Yield(token);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
