using System.Threading;
using UnityEngine;

namespace MainBattleScene
{
    public class MainBattlePlayerCharacterAbilityLaser : MonoBehaviour
    {
        
        [SerializeField] private MainBattlePlayerCharacter _playerCharacter;
        [SerializeField] private float _innerCoolTimeToShotLaser;
        
        CancellationTokenSource _playerShotLaserCancellationTokenSource;
        
        MainBattlePlayerCharacter.PlayerCharacterAbilitySpecialAStats _playerCharacterAbilityShotLaser => _playerCharacter.PlayerAbilitySpecialAStats;
        MainBattlePlayerCharacter.PlayerCharacterBasicStats _playerCharacterBasicStats => _playerCharacter.PlayerBasicStats;
        
    }
}