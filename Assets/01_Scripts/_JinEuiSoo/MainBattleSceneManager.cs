using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainBattleScene
{
    public partial class MainBattleSceneManager : MonoBehaviour
    {
        public static MainBattleSceneManager Instance => _instance;
        static MainBattleSceneManager _instance;
        
        public PlayerManager PlayerManager;
        public BossManager BossManager;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                return;
            }
            // else
            
            
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
    
    


}