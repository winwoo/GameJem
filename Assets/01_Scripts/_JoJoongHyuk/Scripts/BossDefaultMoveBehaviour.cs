using UnityEngine;

public class BossDefaultMoveBehaviour : BossBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;
    private Vector3 _targetPosition;

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
