using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum BattleBugType
{
    PlayerMove,
    PlayerAttack,
    PlayerDash,
    BossMove,
    BossAttack,
    BossHp
}

public class BattleDesignManager : BaseManager
{
    private readonly Dictionary<BattleBugType, bool> _battleBugDic = new Dictionary<BattleBugType, bool>();
    public override async UniTask Init()
    {
        await UniTask.CompletedTask;
    }

    public override async UniTask Dispose()
    {
        await UniTask.CompletedTask;
    }

    public void SetBattleBug(BattleBugType bugType, bool isOn)
    {
        _battleBugDic[bugType] = isOn;
    }
}