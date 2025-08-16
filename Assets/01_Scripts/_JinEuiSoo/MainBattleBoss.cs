using UnityEngine;

namespace MainBattleScene
{

    public class MainBattleBoss : MonoBehaviour
    {
        [SerializeField] public Transform[] AttackPoints;

        [System.Serializable]
        public class BossBasicStats
        {
            [SerializeField] public int MaxHealth;
            [SerializeField] public int CurrentHealth;
            [SerializeField] public float BossMoveSpeed;
            [SerializeField] public bool CanMove = true;
        }

        [System.Serializable]
        public class BossDefaultMoveStats
        {
            [SerializeField] public float MoveSpeed;
            [SerializeField] public float RotationSpeed;

            [SerializeField] public float RetargetTime;
        }

        [System.Serializable]
        public class BossNormalAttackStats
        {
            [SerializeField] public BossProjectile[] BossProjectiles;
            [SerializeField] public float AttackRange;
            [SerializeField] public float AttackCooldown;
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
            Destroy(gameObject);
        }
    }
}