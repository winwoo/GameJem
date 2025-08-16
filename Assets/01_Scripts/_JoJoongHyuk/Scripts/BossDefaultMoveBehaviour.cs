using MainBattleScene;
using UnityEngine;

public class BossDefaultMoveBehaviour : BossBehaviour
{
    Vector3 _targetPosition;
    float _currentTime;

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
        _currentTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.MoveSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(_targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RotationSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 1f || _currentTime > MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RetargetTime)
        {
            _targetPosition = GetRandomPosition();
            _currentTime = 0f;
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-15f, 15f);
        float z = Random.Range(-15f, 15f);
        return new Vector3(x, transform.position.y, z);
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
    }
}
