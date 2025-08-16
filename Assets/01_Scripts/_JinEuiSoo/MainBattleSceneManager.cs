using Cysharp.Threading.Tasks;
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

        [SerializeField] private bool _endBattleTest;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;

                goto InitializeSectionStart;
            }
            // else

            {
                var go = GameObject.Find("MainBattleSceneManager");

                if (go != null)
                {
                    Destroy(go.gameObject);
                }
                
                _instance = this;

                goto InitializeSectionStart;

            }
            
            InitializeSectionStart: ;
            

        }

        void Start()
        {
            StartUpdate();
        }

        void Update()
        {
            UpdateUpdate();

            if (_endBattleTest == true)
            {
                ReportEndBattle();
            }
        }

        // Condition, Boss or Player Health 0
        public void ReportEndBattle()
        {
            Managers.Scene.LoadSceneAsync(Define.Scene.Lobby).Forget();
        }
        
    }
    
    


}