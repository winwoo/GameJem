using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum BattlePlayDesginType
{
    Dash = 0,
    Special1 = 1,
    Special2 = 2,
}

public enum BattleBossDesignType
{
    Special1 = 0,
    Special2 = 1,
    Special3 = 2,
}

public class BattleDesignManager : BaseManager
{
    public readonly HashSet<BattlePlayDesginType> PlayerDesignForlder = new HashSet<BattlePlayDesginType>
    {
        BattlePlayDesginType.Dash,
        BattlePlayDesginType.Special1,
        BattlePlayDesginType.Special2,
    };

    public readonly HashSet<BattleBossDesignType> BossDesignForlder = new HashSet<BattleBossDesignType>
    {
        BattleBossDesignType.Special1,
        BattleBossDesignType.Special2,
        BattleBossDesignType.Special3,
    };

    public readonly HashSet<BattlePlayDesginType> PlayerDesignApplied = new();
    public readonly HashSet<BattleBossDesignType> BossDesignApplied = new();
    public override async UniTask Init()
    {

        await UniTask.CompletedTask;
    }

    public override async UniTask Dispose()
    {
        await UniTask.CompletedTask;
    }
}