using UnityEngine;

namespace MainBattleScene
{
    public class MainBattlePlayerCharacter : MonoBehaviour
    {
        [SerializeField] private MainBattleSceneManager _manager;

        [SerializeField] private Rigidbody _rigidbody; 
        
        private void Update()
        {

            Vector3 inputVetor = new Vector3();

            if (Input.GetKey(KeyCode.W) == true)
            {
                inputVetor.y += 1;
            }
            
            if (Input.GetKey(KeyCode.S) == true)
            {
                inputVetor.y -= 1;
            }
            
            if (Input.GetKey(KeyCode.D) == true)
            {
                inputVetor.x += 1;
            }
            
            if (Input.GetKey(KeyCode.A) == true)
            {
                inputVetor.x -= 1;
            }

            inputVetor *= 1f;
            
            _rigidbody.linearVelocity = inputVetor;
            
            
            
        }
        
        [System.Serializable]
        public class PlayerCharacterStats
        {
            [SerializeField] public int MaxHealth;
            [SerializeField] public int CurrentHealth;
        }
    }
}