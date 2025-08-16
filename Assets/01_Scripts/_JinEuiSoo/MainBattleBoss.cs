using UnityEngine;

namespace MainBattleScene
{

    public class MainBattleBoss : MonoBehaviour
    {
        [SerializeField] public Transform[] AttackPoints;

        [System.Serializable]
        public class BossBasicStats
        {
            [Header("NormalMode")]
            [SerializeField] public int MaxHealth;
            [SerializeField] public int CurrentHealth;

            [Header("BugMode")]
            [SerializeField] public int BugMaxHealth;
            [SerializeField] public int BugCurrentHealth;
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
            MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth -= damage;
            if (MainBattleSceneManager.Instance.BossManager.BossBasicStats.CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"Boss {this.GetType().Name} has died.");
            MainBattleSceneManager.Instance.BossManager.RemoveAllBossBehaviours();
            Destroy(gameObject);
        }
    }
}