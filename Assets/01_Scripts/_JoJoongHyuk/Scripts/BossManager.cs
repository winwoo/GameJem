using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MainBattleScene;
using Unity.VisualScripting;
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
    [Header("Boss Audio Clip Names")]
    public string BossStartAudioClipName;
    public string BossHitAudioClipName;
    public string BossDeathAudioClipName;
    public string BossAttack1AudioClipName;
    public string BossAttack2AudioClipName;

    [Space(10)]    
    public MainBattleBoss Boss;

    public MainBattleBoss.BossBasicStats BossBasicStats;
    public MainBattleBoss.BossDefaultMoveStats BossDefaultMoveStats;
    public MainBattleBoss.BossNormalAttackStats BossNormalAttackStats;

    [SerializeField]
    private List<BossBehaviour> _currentActiveBossBehaviours = new List<BossBehaviour>();

    public Transform ToBeRemovedTransform;

    private void Start()
    {
        if (Managers.Instance.IsBug(BattleBugType.BossHp))
        {
            BossBasicStats.IsBugMode = true;
        }

        for (int i = 0; i < 2; i++)
        {
            AddBossBehaviour(i);
        }

        Managers.Sound.PlaySFX(BossStartAudioClipName).Forget();
    }

    void Update()
    {
        foreach (var behaviour in _currentActiveBossBehaviours)
        {
            behaviour.UpdateBehaviour();
        }
    }

    public void AddBossBehaviour(int index)
    {
        BossBehaviour behaviour = null;

        switch (index)
        {
            case 0:
                behaviour = Boss.gameObject.AddComponent<BossDefaultMoveBehaviour>();
                behaviour.IsBugMode = Managers.Instance.IsBug(BattleBugType.BossMove);
                break;
            case 1:
                behaviour = Boss.gameObject.AddComponent<BossNormalAttackBehaviour>();
                behaviour.IsBugMode = Managers.Instance.IsBug(BattleBugType.BossAttack);
                break;
            default:
                Debug.LogWarning("Unknown Boss Behaviour Type");
                break;
        }

        if (behaviour != null)
        {
            _currentActiveBossBehaviours.Add(behaviour);
            behaviour.PlayBehaviour();
        }
    }

    public void RemoveBossBehaviour(BossBehaviour behaviour)
    {
        if (_currentActiveBossBehaviours.Contains(behaviour))
        {
            behaviour.StopBehaviour();
            _currentActiveBossBehaviours.Remove(behaviour);
            Destroy(behaviour);
        }
    }

    public void RemoveAllBossBehaviours()
    {
        foreach (var behaviour in _currentActiveBossBehaviours)
        {
            behaviour.StopBehaviour();
            Destroy(behaviour);
        }
        _currentActiveBossBehaviours.Clear();
        

    }
}