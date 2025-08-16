using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HitText : MonoBehaviour
{
    [SerializeField] private string _positiveColorHex = "#00FF00"; // Green
    [SerializeField] private string _negativeColorHex = "#FF0000"; // Red
    [SerializeField] private TextMeshProUGUI _text;

    public void ShowHitText(bool isPositive)
    {
        string colorHex = isPositive ? _positiveColorHex : _negativeColorHex;
        _text.text = isPositive ? $"<color={colorHex}>+</color>" : $"<color={colorHex}>-</color>";
        AnimateAndDestroyAsync().Forget();
    }

    private async Cysharp.Threading.Tasks.UniTaskVoid AnimateAndDestroyAsync()
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 50f, 0); // 50 units 위로 이동
        Color startColor = _text.color;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            _text.color = c;
            elapsed += Time.deltaTime;
            await Cysharp.Threading.Tasks.UniTask.Yield();
        }
        Destroy(gameObject);
    }
}
