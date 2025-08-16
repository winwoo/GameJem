using UnityEngine;

namespace MainBattleScene
{

    public class MainBattleBoss : MonoBehaviour
    {
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
            [SerializeField]
            private float _moveSpeed = 5f;
        }
        

    }
}