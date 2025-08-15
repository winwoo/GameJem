using Unity.VisualScripting;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public virtual void Awake()
    {
        // Initialize the boss
        InternalInitialize();
    }

    private void InternalInitialize()
    {
        Debug.Log($"Boss {this.GetType().Name} has initialized.");
    }

    public virtual void PlayBehaviour()
    {
        Debug.Log($"Boss {this.GetType().Name} is playing its behaviour.");
    }

    public virtual void UpdateBehaviour()
    {
        Debug.Log($"Boss {this.GetType().Name} is updating its behaviour.");
    }

    public virtual void StopBehaviour()
    {
        Debug.Log($"Boss {this.GetType().Name} has stopped its behaviour.");
    }
}
