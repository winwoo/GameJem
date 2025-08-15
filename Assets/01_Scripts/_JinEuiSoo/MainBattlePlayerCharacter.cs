using System;
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
        
        PlayerCharacterBasicStats _playerBasicBasicStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterBasicStats;
        PlayerCharacterAttackStats _playerAttackStats => MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterAttackStats;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {

            #region PlayerMovement

            if (_playerBasicBasicStats.CanMove == false)
            {
                goto LastOfPlayerMovement;
            }
            
            Vector3 inputVetor = new Vector3();

            if (Input.GetKey(KeyCode.W) == true)
            {
                inputVetor.z += 1;
            }
            
            if (Input.GetKey(KeyCode.S) == true)
            {
                inputVetor.z -= 1;
            }
            
            if (Input.GetKey(KeyCode.D) == true)
            {
                inputVetor.x += 1;
            }
            
            if (Input.GetKey(KeyCode.A) == true)
            {
                inputVetor.x -= 1;
            }

            inputVetor *= MainBattleSceneManager.Instance.PlayerManager.PlayerCharacterBasicStats.PlayerMoveSpeed;
            
            _rigidbody.linearVelocity = inputVetor;
            
            
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
                transform.position + _playerAttackStats.ProjectileSpawnRelatedPosition;
            projectileGameObject.GetComponent<PlayerAttackProjectile>().Initialize(_directionToMouseHit, 
                _playerAttackStats.ProjectileSpeed, _playerAttackStats.ProjectileDamage, _playerAttackStats.ProjectileLifeTime);
            
            
            
            OutOfPlayerAttacking: ;
            
            #endregion


        }
        
        [System.Serializable]
        public class PlayerCharacterBasicStats
        {
            [SerializeField] public int MaxHealth;
            [SerializeField] public int CurrentHealth;
            [SerializeField] public float PlayerMoveSpeed;
            [SerializeField] public bool CanMove = true;
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
    }
}