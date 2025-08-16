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
            
            transform.Translate(_directionToward * (_speed * Time.deltaTime));
            
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<MainBattleBoss>(out var mainBattleBoss) == false)
            {
                return;
            }
            
            mainBattleBoss.TakeDamage(_damage);
            
        }
    }
}