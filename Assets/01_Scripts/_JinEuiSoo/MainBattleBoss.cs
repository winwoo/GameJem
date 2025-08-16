using System;
using UnityEngine;

namespace MainBattleScene
{

    public class MainBattleBoss : MonoBehaviour
    {
        public Action BossDieAction;
        [SerializeField] public Transform[] AttackPoints;

        [System.Serializable]
        public class BossBasicStats
        {
            [SerializeField] public bool IsBugMode;
            [SerializeField] public int MaxHealth;
            [SerializeField] public int CurrentHealth;
        }

        [System.Serializable]
        public class BossDefaultMoveStats
        {
            [Header("NormalMode")]
            [SerializeField] public float MoveSpeed;
            [SerializeField] public float RotationSpeed;
            [SerializeField] public float RetargetTime;
            [Header("BugMode")]
            [SerializeField] public float BugMoveSpeed;
            [SerializeField] public float BugRotationSpeed;
            [SerializeField] public float BugRetargetTime;

        }

        [System.Serializable]
        public class BossNormalAttackStats
        {
            [SerializeField] public BossProjectile[] BossProjectiles;

            [Header("NormalMode")]
            [SerializeField] public float AttackRange;
            [SerializeField] public float AttackCooldown;
            [SerializeField] public float ProjectileSpeed;
            [Header("BugMode")]
            [SerializeField] public float BugAttackRange;
            [SerializeField] public float BugAttackCooldown;
            [SerializeField] public float BugProjectileSpeed;

        }

        public void TakeDamage(int damage)
        {
            if (MainBattleSceneManager.Instance.BossManager.BossBasicStats.IsBugMode)
            {
                MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth += damage;
            }
            else
            {
                MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth -= damage;
            }

            MainBattleSceneManager.Instance.UpdateBossHealthUI();

            if (MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth <= 0)
            {
                MainBattleSceneManager.Instance.BossHpBar.gameObject.SetActive(false);

                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"Boss {this.GetType().Name} has died.");
            BossDieAction?.Invoke();
            MainBattleSceneManager.Instance.BossManager.RemoveAllBossBehaviours();
            Destroy(gameObject);
        }

#if false
        public void TakeDamage(int damage)
        {
            //플레이어가 공격한 데미지만큼 보스의 체력이 증가하는 코드
            MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth += damage;

            if (MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth <= 0)
            {
                Die();
            }
        }
#endif

#if false
        public void TakeDamage(int damage)
        {
            //플레이어가 공격한 데미지만큼 보스의 체력이 감소되는 코드
            MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth -= damage;

            if (MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth <= 0)
            {
                Die();
            }
        }
#endif
    }
}