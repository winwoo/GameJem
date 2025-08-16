using MainBattleScene;
using UnityEngine;

public class BossDefaultMoveBehaviour : BossBehaviour
{
    Vector3 _targetPosition;
    float _currentTime;
    float _moveSpeed;
    float _rotateSpeed;
    float _retargetTime;

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

        if (IsBugMode)
        {
            _moveSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.BugMoveSpeed;
            _rotateSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.BugRotationSpeed;
            _retargetTime = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.BugRetargetTime;
        }
        else
        {
            _moveSpeed = 0.5f;
            _rotateSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RotationSpeed;
            _retargetTime = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RetargetTime;
        }

        _currentTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(_targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 1f || _currentTime > _retargetTime)
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

    #if false
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();

        //보스 이동 속도 20배 빠른 테스트용 코드
        _moveSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.MoveSpeed;
        _moveSpeed *= 20f;
        _rotateSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RotationSpeed;
        _retargetTime = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RetargetTime;

        _currentTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
    }


    
    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();

        //보스 이동 속도 밸런스 조절된 수치 적용
        _moveSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.MoveSpeedf;
        _rotateSpeed = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RotationSpeed;
        _retargetTime = MainBattleSceneManager.Instance.BossManager.BossDefaultMoveStats.RetargetTime;

        _currentTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
    }
#endif
}
