using UnityEngine;

namespace MainBattleScene
{

    public class MainBattleBoss : MonoBehaviour
    {
        [SerializeField]
        private BossBehaviour[] _bossBehaviours;

        public void PlayBehaviour(BossBehaviourType behaviourType)
        {
            foreach (var bossBehaviour in _bossBehaviours)
            {
                if (bossBehaviour.BehaviourType == behaviourType)
                {
                    bossBehaviour.PlayBehaviour();
                }
            }
        }
    }
}