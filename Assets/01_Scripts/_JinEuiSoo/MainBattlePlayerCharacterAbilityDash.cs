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
            #region Dashing

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
                goto EndOfDashing;
            }

            if (_playerCharacterBasicStats.CanDashing == false)
            {
                // Maybe show notice
                goto EndOfDashing;
            }
            
            // Start Action
            // // Set Condition
            _innerCoolTimeToDashing = _playerCharacterAbilityDashStats.DashCooldown;
            _playerCharacterBasicStats.CanDashing = false;
            
            DashingPlayerCharacter(_playerCharacter.DirectionToMouseHit * _playerCharacterAbilityDashStats.DashPower,
                _playerCharacterAbilityDashStats.DashDuration).Forget();
            
            
            EndOfDashing: ;

            #endregion
        }
        
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
            
#if false // Method Translate
            InitializeDashingCancellationTokenSource();
            var cancellationToken = _playerDashingCancellationTokenSource.Token;

            _playerCharacterBasicStats.CanMove = false;
            _playerCharacterBasicStats.IsDashing = true;
            float innerTime = 0;

            Vector3 anSecondMovePosition = totalRelatedAmountMovePosition / time;
            
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
                
                await UniTask.WaitForSeconds(Time.deltaTime);
                
                Vector3 movePosition = anSecondMovePosition * Time.deltaTime;
                _playerCharacter.MovePlayerCharacter(movePosition);

            }
            
            _playerCharacterBasicStats.CanMove = true;
            _playerCharacterBasicStats.IsDashing = false;
            
#endif
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