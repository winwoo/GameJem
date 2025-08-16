using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Scriptable Objects/DialogData")]
public class DialogData : ScriptableObject
{
    [Header("대화 문장")]
    public List<string> Sentences;
}
