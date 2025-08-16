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
        foreach(InitBugTypeData data in Managers.Instance.InitBugSetting.InitBugData)
        {
            _battleBugDic[data.Type] = data.IsBug;
        }
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