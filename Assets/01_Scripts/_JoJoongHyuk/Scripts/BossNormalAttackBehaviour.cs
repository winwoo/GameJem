using UnityEngine;
using UnityEngine.Rendering;

public class BossNormalAttackBehaviour : BossBehaviour
{
    [SerializeField]
    private float _attackRange = 2f;
    [SerializeField]
    private float _attackCooldown = 2f;

    public override void Awake()
    {
        base.Awake();
    }

    public override void PlayBehaviour()
    {
        base.PlayBehaviour();
    }

    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
    }
}
