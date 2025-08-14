using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public sealed class AudioHost : MonoBehaviour
{
    public CancellationToken Token => this.GetCancellationTokenOnDestroy();
}