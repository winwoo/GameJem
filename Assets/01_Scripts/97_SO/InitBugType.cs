using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InitBugType", menuName = "Scriptable Objects/InitBugType")]
public class InitBugType : ScriptableObject
{
    [SerializeField]
    private InitBugTypeData[] _initBugData;

    public InitBugTypeData[] InitBugData => _initBugData;
}

[Serializable]
public class InitBugTypeData
{
    [Header("버그 타입")]
    [SerializeField]
    private BattleBugType _type;
    [Header("체크 하면 버그 상태")]
    [SerializeField]
    private bool _isBug;
    [Header("폴더 이름")]
    [SerializeField]
    private string _folderName;

    [Header("정상코드 이미지")]
    [SerializeField]
    private Sprite _normalCodeImage;
    [Header("버그 코드 이미지")]
    [SerializeField]
    private Sprite _bugCodeImage;

    public BattleBugType Type => _type;
    public bool IsBug => _isBug;
    public string FolderName => _folderName;
    public Sprite NormalCodeImage => _normalCodeImage;
    public Sprite BugCodeImage => _bugCodeImage;
}
