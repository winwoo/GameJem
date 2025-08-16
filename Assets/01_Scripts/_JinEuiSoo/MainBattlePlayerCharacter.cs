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

            #region PlayerMovement

            if (PlayerBasicStats.CanMove == false)
            {
                goto LastOfPlayerMovement;
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
            
            
            
            LastOfPlayerMovement: ;

            #endregion
            
            #region Set Direction to mouse hit

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 300f) == false)
            {
                goto OutOfFindGround;
            }
            
            // // Set Direction to Mouse hit
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            Vector3 mouseWorldPoint = hit.point;
            
            _directionToMouseHit = mouseWorldPoint - transform.position;
            _directionToMouseHit = Vector3.Normalize(_directionToMouseHit);
            
            OutOfFindGround: ;
            
            #endregion

            #region Player Attacking

            if (Input.GetMouseButtonDown(0) == false)
            {
                goto OutOfPlayerAttacking;
            }
            
            GameObject projectileGameObject = Instantiate(_playerAttackProjectile, _projectileGroup.transform);
            projectileGameObject.transform.position =
                transform.position + PlayerAttackStats.ProjectileSpawnRelatedPosition;
            projectileGameObject.GetComponent<PlayerAttackProjectile>().Initialize(_directionToMouseHit, 
                PlayerAttackStats.ProjectileSpeed, PlayerAttackStats.ProjectileDamage, PlayerAttackStats.ProjectileLifeTime);
            
            
            
            OutOfPlayerAttacking: ;
            
            #endregion


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">Without Multiply deltaTime. Expect Programmer Apply it</param>
        public void MovePlayerCharacter(Vector3 direction)
        {
            direction.y = _rigidbody.linearVelocity.y;;
            
            _rigidbody.linearVelocity = direction;
            
        }

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

            // Check And Report End Battle
            if (PlayerBasicStats.CurrentHealth <= 0)
            {
                CallPlayerDeath();
            }
            
        }

        public void CallPlayerDeath()
        {
            Debug.Log("Player Death");
            PlayerBasicStats.CurrentHealth = 0;
            
            MainBattleSceneManager.Instance.ReportEndBattle();

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