using MainBattleScene;
using UnityEngine;

public enum BossBehaviourType
{
    BossDefaultMoveBehaviour = 0,
    BossNormalAttackBehaviour,
    BossAggressiveAttackBehaviour,
    BossDefensiveStanceBehaviour
}

public class BossManager : MonoBehaviour
{
    public MainBattleBoss Boss;

    public MainBattleBoss.BossBasicStats BossBasicStats;
    public MainBattleBoss.BossDefaultMoveStats BossDefaultMoveStats;

    public void AddBossBehaviour(int index)
    {
        switch (index)
        {
            case 0:
                Boss.gameObject.AddComponent<BossDefaultMoveBehaviour>();
                break;
            case 1:
                gameObject.AddComponent<BossNormalAttackBehaviour>();
                break;
            default:
                Debug.LogWarning("Unknown Boss Behaviour Type");
                break;
        }
    }
}