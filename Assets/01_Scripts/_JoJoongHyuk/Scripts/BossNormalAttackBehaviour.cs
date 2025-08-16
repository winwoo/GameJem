using Cysharp.Threading.Tasks;
using MainBattleScene;
using UnityEngine;
using UnityEngine.Rendering;

public class BossNormalAttackBehaviour : BossBehaviour
{
    Transform[] _attackPoints;
    BossProjectile[] _bossProjectiles;

    public override void Awake()
    {
        base.Awake();
    }

    public override void PlayBehaviour()
    {
        base.PlayBehaviour();

        if (_attackPoints == null || _attackPoints.Length == 0)
        {
            _attackPoints = MainBattleSceneManager.Instance.BossManager.Boss.AttackPoints;
        }

        if (_bossProjectiles == null || _bossProjectiles.Length == 0)
        {
            _bossProjectiles = MainBattleSceneManager.Instance.BossManager.BossNormalAttackStats.BossProjectiles;
        }

        FireProjectile().Forget();
    }

    public override void UpdateBehaviour()
    {
        base.UpdateBehaviour();
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
    }

    private async UniTask FireProjectile()
    {
        while (true)
        {
            if (_bossProjectiles == null || _bossProjectiles.Length == 0 || _attackPoints == null || _attackPoints.Length == 0)
            {
                Debug.LogWarning("No projectiles or attack points available for Boss Normal Attack Behaviour.");
                return;
            }

            await UniTask.Delay((int)(MainBattleSceneManager.Instance.BossManager.BossNormalAttackStats.AttackCooldown * 1000));
            SpawnProjectile();
        }
    }
    
    private void SpawnProjectile()
    {
        Debug.Log($"Spawning projectile for {this.GetType().Name}");
        BossProjectile randomProjectile = _bossProjectiles[Random.Range(0, _bossProjectiles.Length)];
        Transform randomAttackPoint = _attackPoints[Random.Range(0, _attackPoints.Length)];

        if (randomProjectile.IsFixedPosition)
        {
            var spawnedProjectile = Instantiate(randomProjectile, randomAttackPoint.transform.position, randomAttackPoint.transform.rotation).GetComponent<BossProjectile>();
        }
        else
        {
            var spawnedProjectile = Instantiate(randomProjectile, randomAttackPoint.transform.position, randomAttackPoint.transform.rotation).GetComponent<BossProjectile>();

            Vector3 direction = randomAttackPoint.position - transform.position;
            direction.y = transform.position.y;
            direction.Normalize();
            spawnedProjectile.SetDirection(direction);
        }
        
    }
}
