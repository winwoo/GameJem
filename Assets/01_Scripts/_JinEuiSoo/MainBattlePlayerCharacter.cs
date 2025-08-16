using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEngine;

namespace MainBattleScene
{
    public class MainBattlePlayerCharacter : MonoBehaviour
    {
        [SerializeField] private MainBattleSceneManager _manager;
        [SerializeField] private Rigidbody _rigidbody; 
        [SerializeField] private GameObject _playerAttackProjectile;
        [SerializeField] private GameObject _projectileGroup;
        [SerializeField] private Animator _animator;
        
        [Header("Debug")] 
        [SerializeField] private Vector3 _directionToMouseHit; 
        [SerializeField] Camera _camera;
        
        public Vector3 DirectionToMouseHit => _directionToMouseHit + PlayerBasicStats.MouseHitAdjust;
        
        public PlayerCharacterBasicStats PlayerBasicStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterBasicStats;
        public PlayerCharacterAttackStats PlayerAttackStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterAttackStats;
        public PlayerCharacterAbilityDashStats PlayerAbilityDashStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterAbilityDashStats;
        public PlayerCharacterAbilitySpecialAStats PlayerAbilitySpecialAStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterAbilitySpecialAStats;
        public PlayerCharacterAbilitySpecialBStats PlayerAbilitySpecialBStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterAbilitySpecialBStats;
        
        CancellationTokenSource _playerKnockBackCancellationTokenSource;
        
        
        

        private void Start()
        {
            _camera = Camera.main;
            
            _manager = MainBattleSceneManager.Instance;
            _projectileGroup = _manager.transform.GetChild(0).gameObject;
        }

        private void Update()
        {

            UpdatePlayerMovement();
            SetDirectionToMouseHit();
            ActionPlayerAttack();

            SetPlayerRunOrIdleAnimation();
        }

        void SetPlayerRunOrIdleAnimation()
        {
            Vector3 playerCurrentVelocity = _rigidbody.linearVelocity;
            
            SetPlayerRotation(playerCurrentVelocity);
            
            // Set Animations
            if(playerCurrentVelocity.sqrMagnitude > 0.05f)
            {
                _animator.SetBool("RunBool", true);
            }
            else
            {
                _animator.SetBool("RunBool", false);
            }
            
        }

        void SetPlayerRotation(Vector3 playerCurrentVelocity)
        {
            if (playerCurrentVelocity.sqrMagnitude < 0.1f)
            {
                return;
            }
            
            Vector3 playerMoveDirection = playerCurrentVelocity;
            playerMoveDirection.y = 0f;
            playerMoveDirection = playerMoveDirection.normalized;
            Quaternion newRotation = Quaternion.LookRotation(playerMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10f);
        }

        void UpdatePlayerMovement()
        {
            if (PlayerBasicStats.CanMove == false)
            {
                return;
            }
            
            Vector3 inputVector = new Vector3();

            if (Input.GetKey(KeyCode.W) == true)
            {
                inputVector.z += 1;
            }
            
            if (Input.GetKey(KeyCode.S) == true)
            {
                inputVector.z -= 1;
            }
            
            if (Input.GetKey(KeyCode.D) == true)
            {
                inputVector.x += 1;
            }
            
            if (Input.GetKey(KeyCode.A) == true)
            {
                inputVector.x -= 1;
            }

            inputVector *= MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterBasicStats.PlayerMoveSpeed;
                
            MovePlayerCharacter(inputVector);;
        }

        void SetDirectionToMouseHit()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 300f) == false)
            {
                return;
            }
            
            // // Set Direction to Mouse hit
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            Vector3 mouseWorldPoint = hit.point;
            
            _directionToMouseHit = mouseWorldPoint - transform.position;
            _directionToMouseHit = Vector3.Normalize(_directionToMouseHit);
        }

        // Debug Case 1 -> Player will attack to the wrong position 
        void ActionPlayerAttack()
        {
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }
            
            GameObject projectileGameObject = Instantiate(_playerAttackProjectile, _projectileGroup.transform);
            projectileGameObject.transform.position =
                transform.position + PlayerAttackStats.ProjectileSpawnRelatedPosition;
            
            Vector3 projectilesDirection = _directionToMouseHit;
            
            if (MainBattleSceneManager.Instance.PlayerFeatureConditions[1] == false)
            {
                projectilesDirection = _camera.transform.position;
            }
            
            projectileGameObject.GetComponent<PlayerAttackProjectile>().Initialize(projectilesDirection, 
                PlayerAttackStats.ProjectileSpeed, PlayerAttackStats.ProjectileDamage, PlayerAttackStats.ProjectileLifeTime);
        }
        
        // Player Feature move Normal <-> Player Feature move Bug
        // Both controlled in this method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">Without Multiply deltaTime. Expect Programmer Apply it</param>
        public void MovePlayerCharacter(Vector3 direction)   
        {
            direction.y = _rigidbody.linearVelocity.y;;
            
            // Bug Feature. When 0 is false, Bug Feature will happen
            if (MainBattleSceneManager.Instance.PlayerFeatureConditions[0] == false)
            {
                direction *= -1f;
            }
            
            _rigidbody.linearVelocity = direction;
            
            
            
        }

#if false // Move Right
        
        
        
        
        
        
        
        
        
        
        public void MovePlayerCharacter(Vector3 direction)   
        {
            // 플레이어 캐릭터의 속도 조절!
            direction.y = _rigidbody.linearVelocity.y;;
            
            // 키보드 화살표 방향으로 이동 시키기!
            _rigidbody.linearVelocity = direction;
            
        }
        
        
        
        
        
        
        
        
        
#endif

#if false // Move - Bug










        public void MovePlayerCharacter(Vector3 direction)   
        {
            // 플레이어 캐릭터의 속도 조절!
            direction.y = _rigidbody.linearVelocity.y;;
            
            // 키보드와 반대 방향으로 이동 시키기!
            _rigidbody.linearVelocity = direction * -1f;
            
        }
        
        
        
        
        
        
        
        
        
#endif

#if false


        private Vector3 _directionToMousePosition;



        void ActionPlayerAttack()
        {
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }
            // 플레이어 캐릭터의 공격을 시작하기!!
            GameObject projectileGameObject = Instantiate(_playerAttackProjectile, _projectileGroup.transform);
            projectileGameObject.transform.position =
                transform.position + PlayerAttackStats.ProjectileSpawnRelatedPosition;
            // 플레이어 캐릭터의 공격 방향을 마우스 위치로 설정하기!!
            Vector3 projectilesDirection = _directionToMousePosition;
            
            projectileGameObject.GetComponent<PlayerAttackProjectile>().
                Initialize(projectilesDirection, 
                PlayerAttackStats.ProjectileSpeed, 
                PlayerAttackStats.ProjectileDamage, 
                PlayerAttackStats.ProjectileLifeTime);
        }

        
        
        
        
        
#endif
        
#if false
        





        private Vector3 _directionToMousePosition;



        void ActionPlayerAttack()
        {
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }
            // 플레이어 캐릭터의 공격을 시작하기!!
            GameObject projectileGameObject = Instantiate(_playerAttackProjectile, _projectileGroup.transform);
            projectileGameObject.transform.position =
                transform.position + PlayerAttackStats.ProjectileSpawnRelatedPosition;
            // 플레이어 캐릭터의 공격 방향을 카메라 위치로 설정하기!!
            Vector3 projectilesDirection = _directionToMousePosition;
            
            projectileGameObject.GetComponent<PlayerAttackProjectile>().
                Initialize(projectilesDirection, 
                PlayerAttackStats.ProjectileSpeed, 
                PlayerAttackStats.ProjectileDamage, 
                PlayerAttackStats.ProjectileLifeTime);
        }

        
        
        
        
        
#endif

        public async UniTask KnockBackPlayerCharacter(Vector3 totalRelatedAmountMovePosition, float time)
        {
            InitializeKnockBackCancellationTokenSource();
            var cancellationToken = _playerKnockBackCancellationTokenSource.Token;

            PlayerBasicStats.CanMove = false;
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
                MovePlayerCharacter(movePosition);

            }
            
            PlayerBasicStats.CanMove = true;
            
        }

        void InitializeKnockBackCancellationTokenSource()
        {
            if (_playerKnockBackCancellationTokenSource != null)
            {
                _playerKnockBackCancellationTokenSource.Cancel();
                _playerKnockBackCancellationTokenSource.Dispose();
                _playerKnockBackCancellationTokenSource = null;
            }
            _playerKnockBackCancellationTokenSource = new CancellationTokenSource();
        }

        public void PlayerCharacterGetDamage(int damage)
        {
            PlayerBasicStats.CurrentHealth -= damage;

            MainBattleSceneManager.Instance.UpdatePlayerHealthUI();

            // Check And Report End Battle
            if (PlayerBasicStats.CurrentHealth <= 0)
            {
                MainBattleSceneManager.Instance.PlayerHpBar.gameObject.SetActive(false);
                CallPlayerDeath();
            }
            
        }

        public void SetDodgeAnimation(bool value)
        {
            _animator.SetBool("DodgeBool", value);

            if (value == false)
            {
                _animator.SetTrigger("DodgeEndTrigger");
            }
        }
        
        public void CallPlayerDeath()
        {
            Debug.Log("Player Death");
            PlayerBasicStats.CurrentHealth = 0;
            
            Destroy(this.gameObject);

        }
        
        [System.Serializable]
        public class PlayerCharacterBasicStats
        {
            [SerializeField] public int MaxHealth;
            [SerializeField] public int CurrentHealth;
            [SerializeField] public float PlayerMoveSpeed;
            
            [Header("MouseHitAdjust")]
            [SerializeField] public Vector3 MouseHitAdjust;
            
            [Header("Debug")]
            [SerializeField] public bool CanMove = true;
            [SerializeField] public bool CanDashing = true;
            [SerializeField] public bool IsDashing = false;
        }

        [System.Serializable]
        public class PlayerCharacterAttackStats
        {
            [Header("Projectile")]
            [SerializeField] public float ProjectileSpeed;
            [SerializeField] public int ProjectileDamage;
            [SerializeField] public float ProjectileLifeTime;
            [SerializeField] public Vector3 ProjectileSpawnRelatedPosition;
            
        }

        [System.Serializable]
        public class PlayerCharacterAbilityDashStats
        {
            [SerializeField] public float DashPower;
            [SerializeField] public float DashDuration;
            [SerializeField] public float DashCooldown;

            [SerializeField] public float DashBugPower;
            [SerializeField] public float DashBugDuration;
            [SerializeField] public float DashBugCooldown;
        }
        
        [System.Serializable]
        public class PlayerCharacterAbilitySpecialAStats
        {
            [SerializeField] public int LaserPower;
            [SerializeField] public float LaserDuration;
            [SerializeField] public float LaserCooldown; 
        }
        
        [System.Serializable]
        public class PlayerCharacterAbilitySpecialBStats
        {
            
        }
    }
}