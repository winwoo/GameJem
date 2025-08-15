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
            
            if (Input.GetKey(KeyCode.W) == true)
            {
                
            }
            
            if (Input.GetKey(KeyCode.W) == true)
            {
                
            }
            
            
            
            
        }
    }
}