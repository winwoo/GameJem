using Cysharp.Threading.Tasks;
using MainBattleScene;
using UnityEngine;
using UnityEngine.Rendering;

public class BossNormalAttackBehaviour : BossBehaviour
{
    Transform[] _attackPoints;
    BossProjectile[] _bossProjectiles;
    private float _attackCooldown;
    private float _projectileSpeed;

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

        if (IsBugMode)
        {
            _attackCooldown = MainBattleSceneManager.Instance.BossManager.BossNormalAttackStats.BugAttackCooldown;
            _projectileSpeed = MainBattleSceneManager.Instance.BossManager.BossNormalAttackStats.BugProjectileSpeed;
        }
        else
        {
            _attackCooldown = MainBattleSceneManager.Instance.BossManager.BossNormalAttackStats.AttackCooldown;
            _projectileSpeed = MainBattleSceneManager.Instance.BossManager.BossNormalAttackStats.ProjectileSpeed;
        }
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

            await UniTask.Delay((int)(_attackCooldown * 1000));
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
            foreach (var attackPoint in _attackPoints)
            {
                var spawnedProjectile = Instantiate(randomProjectile, attackPoint.position, attackPoint.rotation).GetComponent<BossProjectile>();
                spawnedProjectile.Speed = _projectileSpeed;
                Vector3 direction = attackPoint.position - transform.position;
                direction.y = transform.position.y;
                direction.Normalize();
                spawnedProjectile.SetDirection(direction);
            }
            //var spawnedProjectile = Instantiate(randomProjectile, randomAttackPoint.transform.position, randomAttackPoint.transform.rotation).GetComponent<BossProjectile>();
        }
        else
        {
            foreach (var attackPoint in _attackPoints)
            {
                var spawnedProjectile = Instantiate(randomProjectile, attackPoint.position, attackPoint.rotation).GetComponent<BossProjectile>();
                spawnedProjectile.Speed = _projectileSpeed;
                Vector3 direction = attackPoint.position - transform.position;
                direction.y = transform.position.y;
                direction.Normalize();
                spawnedProjectile.SetDirection(direction);
            }
        }

    }

#if false
    
    
    
    
    
    
    
    private async UniTask FireProjectile()
    {
        while (true)
        {
            if (_bossProjectiles == null || _bossProjectiles.Length == 0 || _attackPoints == null || _attackPoints.Length == 0)
            {
                Debug.LogWarning("No projectiles or attack points available for Boss Normal Attack Behaviour.");
                return;
            }
            
            //Boss 투사체 0.1초당 한발씩 발사하도록 셋팅하기 
            _attackCooldown = 0.1f;
            await UniTask.WaitForSeconds(_attackCooldown);
            SpawnProjectile();
        }
    }
    
    
    
    
    


#endif
    
#if false
    
    
    

    private async UniTask FireProjectile()
    {
        while (true)
        {
            if (_bossProjectiles == null || _bossProjectiles.Length == 0 || _attackPoints == null || _attackPoints.Length == 0)
            {
                Debug.LogWarning("No projectiles or attack points available for Boss Normal Attack Behaviour.");
                return;
            }
            
            //Boss 투사체 1초당 한발씩 발사하도록 셋팅하기
            _attackCooldown = 1f;
            await UniTask.WaitForSeconds(_attackCooldown);
            SpawnProjectile();
        }
    }

    
    
    
#endif
}
