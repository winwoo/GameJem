using System;
using UnityEngine;

namespace MainBattleScene
{
    public class PlayerAttackProjectile : MonoBehaviour
    {
        [SerializeField] private Vector3 _directionToward;
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;
        
        [SerializeField] private float _lifeTime;

        [SerializeField] private bool _isInitialize;
        
        
        [SerializeField] ParticleSystem _projectileEffectParticleSystem;
        [SerializeField] GameObject _projectileExplosionEffect;

        private bool _canTranslate = true;
        
        public void Initialize(Vector3 directionToward, float speed, int damage, float lifeTime)
        {
            _directionToward = directionToward;
            _directionToward.y = 0;
            _speed = speed;
            _damage = damage;
            _lifeTime = lifeTime;

            _isInitialize = true;

        }

        private void Update()
        {
            if (_isInitialize == false)
            {
                return;
            }
            
            _lifeTime -= Time.deltaTime;

            if (_lifeTime < 0)
            {
                Destroy(this.gameObject);
            }

            if (_canTranslate == true)
            {
                transform.Translate(_directionToward * (_speed * Time.deltaTime));
            }
            
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<MainBattlePlayerCharacter>(out var mainBattlePlayerCharacter) == true)
            {
                return;
            }
            
            if (other.TryGetComponent<MainBattleBoss>(out var mainBattleBoss) == true)
            {
                mainBattleBoss.TakeDamage(_damage);
            }

            GameObject explosionEffectGo = Instantiate(_projectileExplosionEffect);
            explosionEffectGo.transform.position = transform.position;
            
            var emissionModule = _projectileEffectParticleSystem.emission;
            emissionModule.enabled = false;

            _canTranslate = false;
            
            Destroy(explosionEffectGo, 10f);
            Destroy(this.gameObject, 0.35f);
            
        }
    }
}