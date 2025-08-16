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

            #region End Battle Condition Check

            if (PlayerManager.PlayerCharacterBasicStats.CurrentHealth <= 0)
            {
                _endBattleTest = true;
            }

            if (BossManager.BossBasicStats.CurrentHealth <= 0)
            {
                _endBattleTest = true;
            }

            if (_endBattleTest == true)
            {
                _endBattleTest = false;
                ReportEndBattle();
            }

            #endregion
        }

        // Condition, Boss or Player Health 0
        public async void ReportEndBattle()
        {
            await Managers.UI.Open<UISteam>();
            await Managers.Scene.LoadSceneAsync(Define.Scene.Title);
        }
        
    }
    
    


}