using System;
using UnityEngine;

namespace MainBattleScene
{
    public class PlayerManager : MonoBehaviour
    {
        public MainBattlePlayerCharacter.PlayerCharacterBasicStats PlayerCharacterBasicStats;
        public MainBattlePlayerCharacter.PlayerCharacterAttackStats PlayerCharacterAttackStats;
        public MainBattlePlayerCharacter.PlayerCharacterAbilityDashStats PlayerCharacterAbilityDashStats;
        public MainBattlePlayerCharacter.PlayerCharacterAbilitySpecialAStats PlayerCharacterAbilitySpecialAStats;
        public MainBattlePlayerCharacter.PlayerCharacterAbilitySpecialBStats PlayerCharacterAbilitySpecialBStats;
        
        public MainBattlePlayerCharacter PlayerCharacter;

        private void Start()
        {
            PlayerCharacter = GameObject.Find("PlayerCharacter").GetComponent<MainBattlePlayerCharacter>();
        }
    }
}