using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MainBattleScene
{
    public class MainBattlePlayerCharacterAbilityDash : MonoBehaviour
    {
        [SerializeField] private MainBattlePlayerCharacter _playerCharacter;
        [SerializeField] private float _innerCoolTimeToDashing;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private ParticleSystem.EmissionModule _playerDashingParticleSystemEmissionModule;
        
        CancellationTokenSource _playerDashingCancellationTokenSource;
        
        MainBattlePlayerCharacter.PlayerCharacterAbilityDashStats _playerCharacterAbilityDashStats => _playerCharacter.PlayerAbilityDashStats;
        MainBattlePlayerCharacter.PlayerCharacterBasicStats _playerCharacterBasicStats => _playerCharacter.PlayerBasicStats;
        
        private void Start()
        {
            _playerCharacter = this.GetComponent<MainBattlePlayerCharacter>();
            
            _playerDashingParticleSystemEmissionModule = _particleSystem.emission;
            _playerDashingParticleSystemEmissionModule.enabled = false;
            
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

            
            int randomSFX = Random.Range(0, 2);

            switch (randomSFX)
            {
                default:
                case 0:
                    Managers.Sound.PlaySFX("Sound/Player Dash/Sword Woosh 3",volume: 1f);
                    break;
                
                case 1:
                    Managers.Sound.PlaySFX("Sound/Player Dash/Sword Woosh 10",volume: 1f);
                    break;
                
            }
            
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
            _playerDashingParticleSystemEmissionModule.enabled = true;

            _playerCharacterBasicStats.CanMove = false;
            _playerCharacterBasicStats.IsDashing = true;
            float innerTime = 0;
            
            _playerCharacter.MovePlayerCharacter(dashingVector);
            _playerCharacter.SetDodgeAnimation(true);
            
            // Maximum 1000 frame, expected as 10~20 sec
            for (int i = 0; i < 1000; i++)
            {
                await UniTask.WaitForEndOfFrame();
                
                if (cancellationToken.IsCancellationRequested == true)
                {
                    return;
                }
                
                innerTime += Time.deltaTime;
                
                if (innerTime > time)
                {
                    break;
                }
                
                // Else, Action

            }
            
            _playerCharacterBasicStats.CanMove = true;
            _playerCharacterBasicStats.IsDashing = false;
            _playerDashingParticleSystemEmissionModule.enabled = false;
            _playerCharacter.SetDodgeAnimation(false);
            
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

        void OnDestroy()
        {
            if (_playerDashingCancellationTokenSource == null)
            {
                return;
            }
            
            _playerDashingCancellationTokenSource.Cancel();
            _playerDashingCancellationTokenSource.Dispose();
            _playerDashingCancellationTokenSource = null;
        }
        
    }
}