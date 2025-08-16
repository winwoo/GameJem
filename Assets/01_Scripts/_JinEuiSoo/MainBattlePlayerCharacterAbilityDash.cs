using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MainBattleScene
{
    public class MainBattlePlayerCharacterAbilityDash : MonoBehaviour
    {
        [SerializeField] private MainBattlePlayerCharacter _playerCharacter;
        [SerializeField] private float _innerCoolTimeToDashing;
        
        CancellationTokenSource _playerDashingCancellationTokenSource;
        
        MainBattlePlayerCharacter.PlayerCharacterAbilityDashStats _playerCharacterAbilityDashStats => _playerCharacter.PlayerAbilityDashStats;
        MainBattlePlayerCharacter.PlayerCharacterBasicStats _playerCharacterBasicStats => _playerCharacter.PlayerBasicStats;
        
        private void Start()
        {
            _playerCharacter = this.GetComponent<MainBattlePlayerCharacter>();
        }
        
        private void Update()
        {
            ActionPlayerDashing();
        }

        // DashBug Condition
        void ActionPlayerDashing()
        {
            if (_playerCharacterBasicStats.CanDashing == false)
            {
                _innerCoolTimeToDashing -= Time.deltaTime;

                if (_innerCoolTimeToDashing <= 0)
                {
                    _playerCharacterBasicStats.CanDashing = true;
                }
                
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift) == false)
            {
                return;
            }

            if (_playerCharacterBasicStats.CanDashing == false)
            {
                // Maybe show notice
                return;
            }
            
            // Start Action
            // // Set Condition
            _innerCoolTimeToDashing = _playerCharacterAbilityDashStats.DashCooldown;
            _playerCharacterBasicStats.CanDashing = false;
            
            float dashPower = _playerCharacterAbilityDashStats.DashPower;
            float dashDuration = _playerCharacterAbilityDashStats.DashDuration;

            if (MainBattleSceneManager.Instance.PlayerFeatureConditions[2] == false) // bug Condition;
            {
                dashPower = _playerCharacterAbilityDashStats.DashBugPower;
                dashDuration = _playerCharacterAbilityDashStats.DashBugDuration;
            }
            
            DashingPlayerCharacter(_playerCharacter.DirectionToMouseHit * dashPower,
                dashDuration).Forget();
            
            
        }

#if false // Dashing Right Condition
        void ActionPlayerDashing()
        {
            if (_playerCharacterBasicStats.CanDashing == false)
            {
                _innerCoolTimeToDashing -= Time.deltaTime;

                if (_innerCoolTimeToDashing <= 0)
                {
                    _playerCharacterBasicStats.CanDashing = true;
                }
                
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift) == false)
            {
                return;
            }

            if (_playerCharacterBasicStats.CanDashing == false)
            {
                // Maybe show notice
                return;
            }
            
            // Start Action
            // // Set Condition
            _innerCoolTimeToDashing = _playerCharacterAbilityDashStats.DashCooldown;
            _playerCharacterBasicStats.CanDashing = false;
            
            
            // Dash Power 를 기본 값으로 설정하기!!
            float dashPower = _playerCharacterAbilityDashStats.DashPower;
            float dashDuration = _playerCharacterAbilityDashStats.DashDuration;
            
            // PlayerCharacter 대쉬 시작하기!!
            DashingPlayerCharacter(_playerCharacter.DirectionToMouseHit 
                                   * dashPower, dashDuration).Forget();
            
            
        }
        
        
        
        
        
#endif

        
        
        
        
#if false // Dashing Bug Condition
        void ActionPlayerDashing()
        {
            if (_playerCharacterBasicStats.CanDashing == false)
            {
                _innerCoolTimeToDashing -= Time.deltaTime;

                if (_innerCoolTimeToDashing <= 0)
                {
                    _playerCharacterBasicStats.CanDashing = true;
                }
                
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift) == false)
            {
                return;
            }

            if (_playerCharacterBasicStats.CanDashing == false)
            {
                // Maybe show notice
                return;
            }
            
            // Start Action
            // // Set Condition
            _innerCoolTimeToDashing = _playerCharacterAbilityDashStats.DashCooldown;
            _playerCharacterBasicStats.CanDashing = false;
            
            // Dash Power 를 곱셈 값으로 설정하기!!
            float dashPower = _playerCharacterAbilityDashStats.DashPower * 15f;
            float dashDuration = _playerCharacterAbilityDashStats.DashDuration * 20f;
            
            // PlayerCharacter 대쉬 시작하기!!
            DashingPlayerCharacter(_playerCharacter.DirectionToMouseHit 
                                   * dashPower, dashDuration).Forget();
            
            
        }
        
        
        
        
#endif
        
        
        
        
        public async UniTask DashingPlayerCharacter(Vector3 dashingVector, float time)
        {
            InitializeDashingCancellationTokenSource();
            var cancellationToken = _playerDashingCancellationTokenSource.Token;

            _playerCharacterBasicStats.CanMove = false;
            _playerCharacterBasicStats.IsDashing = true;
            float innerTime = 0;
            
            _playerCharacter.MovePlayerCharacter(dashingVector);
            
            // Maximum 1000 frame, expected as 10~20 sec
            for (int i = 0; i < 1000; i++)
            {
                if (cancellationToken.IsCancellationRequested == true)
                {
                    break;
                }
                
                innerTime += Time.deltaTime;
                
                if (innerTime > time)
                {
                    break;
                }
                
                // Else, Action
                await UniTask.WaitForEndOfFrame();

            }
            
            _playerCharacterBasicStats.CanMove = true;
            _playerCharacterBasicStats.IsDashing = false;
            
        }
        
        void InitializeDashingCancellationTokenSource()
        {
            if (_playerDashingCancellationTokenSource != null)
            {
                _playerDashingCancellationTokenSource.Cancel();
                _playerDashingCancellationTokenSource.Dispose();
                _playerDashingCancellationTokenSource = null;
            }
            _playerDashingCancellationTokenSource = new CancellationTokenSource();
        }
        
    }
}