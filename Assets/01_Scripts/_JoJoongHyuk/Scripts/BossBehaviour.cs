using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

public enum BossBehaviourType
{
    BossDefaultMoveBehaviour = 0,
    BossNormalAttackBehaviour,
    BossAggressiveAttackBehaviour,
    BossDefensiveStanceBehaviour
}

public class BossBehaviour : MonoBehaviour
{
public BossBehaviourType BehaviourType { get; private set; }

    public virtual void Awake()
    {
        // Initialize the boss
        InternalInitialize();
    }

    private void InternalInitialize()
    {
        BehaviourType = (BossBehaviourType)System.Enum.Parse(typeof(BossBehaviourType), this.GetType().Name);

        Debug.Log($"Boss {this.name} has initialized.");
    }

    public virtual void PlayBehaviour()
    {
        Debug.Log($"Boss {this.name} is playing its behaviour.");
    }

    public virtual void UpdateBehaviour()
    {
        Debug.Log($"Boss {this.name} is updating its behaviour.");
    }

    public virtual void StopBehaviour()
    {
        Debug.Log($"Boss {this.name} has stopped its behaviour.");
    }
}
