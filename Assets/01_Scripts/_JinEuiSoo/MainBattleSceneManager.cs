using System;
using UnityEngine;

namespace MainBattleScene
{
    public partial class MainBattleSceneManager : MonoBehaviour
    {
        public static MainBattleSceneManager Instance => _instance;
        static MainBattleSceneManager _instance;

        public PlayerCharacterStats PlayerCharacterStats;
        
        private void Awake()
        {

            var go = GameObject.Find("MainBattleSceneManager");

            if (go != null)
            {
                Destroy(go.gameObject);
            }
            
            _instance = this;
            
        }

        void Start()
        {
            StartUpdate();
        }

        void Update()
        {
            UpdateUpdate();
        }
    }
    
    
    [System.Serializable]
    public class PlayerCharacterStats
    {
        [SerializeField] public int MaxHealth;
        [SerializeField] public int CurrentHealth;
    }

}