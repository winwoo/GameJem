using MainBattleScene;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
        [SerializeField] private Vector3 _directionToward;
        [SerializeField] private float _speed;
        [SerializeField] private int _damage;
        [SerializeField] private float _lifeTime;
        [SerializeField] public bool IsFixedPosition = false;
        private float _fixedY;

        private void Start()
        {
            _fixedY = transform.position.y;
        }

        private void Update()
    {
        _lifeTime -= Time.deltaTime;

        if (_lifeTime < 0)
        {
            //effect or sound play code
            Destroy(this.gameObject);
        }
        // _directionToward = transform.position - MainBattleSceneManager.Instance.BossManager.Boss.transform.position;
        // _directionToward.Normalize();
        if(!IsFixedPosition)
        {
            transform.Translate(_directionToward * (_speed * Time.deltaTime));
            // y값을 고정
            transform.position = new Vector3(transform.position.x, _fixedY, transform.position.z);
        }
    }

        public void SetDirection(Vector3 direction)
        {
            _directionToward = direction;
        }

        private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<MainBattlePlayerCharacter>(out var mainBattlePlayerCharacter) == false)
        {
            return;
        }
        
        mainBattlePlayerCharacter.PlayerCharacterGetDamage(_damage);
    }
}
