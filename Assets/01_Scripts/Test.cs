using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField][Link]
    private Image _testImage;
    [SerializeField][Link]
    private TextMeshProUGUI _testText;
    [SerializeField][Link]
    private Button _testButton;
    [SerializeField][Link("testTrans")]
    private Transform _transfrom;
    [SerializeField][Link("testTrans2")]
    private Transform _transfrom2;


}
