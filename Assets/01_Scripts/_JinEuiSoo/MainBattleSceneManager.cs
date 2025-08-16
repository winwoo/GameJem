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
        [SerializeField] private bool _isBattleEndCalled = false;

        [SerializeField] private bool[] playerFeatureConditions;
        public bool[] PlayerFeatureConditions => playerFeatureConditions;
        [SerializeField] private bool[] _bossFeatureConditions;
        public bool[] BossFeatureConditions => _bossFeatureConditions;

        [Header("Hp UI")]
        public RectTransform PlayerHpBar;
        public UnityEngine.UI.Image PlayerHpBarImage;
        public Transform PlayerHitTextSpawnPoint;
        public RectTransform BossHpBar;
        public UnityEngine.UI.Image BossHpBarImage;
        public Transform BossHitTextSpawnPoint;
        public HitText HitText;

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

        InitializeSectionStart:;

            AwakeUpdate();

        }

        void Start()
        {
            StartUpdate();

            Managers.Sound.PlayBGM("Omega Sector (BossBattle)").Forget();
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
                BossManager.RemoveAllBossBehaviours();
                ReportEndBattle();
            }

            #endregion
            if (PlayerManager.PlayerCharacterBasicStats.CurrentHealth > 0)
            {
                Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerManager.PlayerCharacter.transform.position);
                PlayerHpBar.anchoredPosition = playerScreenPoint += new Vector3(0, 50, 0);
            }

            if (BossManager.BossBasicStats.CurrentHealth > 0)
            {
                Vector3 bossScreenPoint = Camera.main.WorldToScreenPoint(BossManager.Boss.transform.position);
                BossHpBar.anchoredPosition = bossScreenPoint += new Vector3(0, 150, 0);
            }
        }

        public void UpdatePlayerHealthUI()
        {
            PlayerHpBarImage.fillAmount = (float)PlayerManager.PlayerCharacterBasicStats.CurrentHealth / PlayerManager.PlayerCharacterBasicStats.MaxHealth;
        }

        public void UpdateBossHealthUI()
        {
            BossHpBarImage.fillAmount = (float)BossManager.BossBasicStats.CurrentHealth / BossManager.BossBasicStats.MaxHealth;
        }

        // Condition, Boss or Player Health 0
        public async void ReportEndBattle()
        {
            if (_isBattleEndCalled == true)
            {
                return;
            }

            _isBattleEndCalled = true;
            await Managers.UI.Open<UISteam>();
            await Managers.Scene.LoadSceneAsync(Define.Scene.Title);
        }

    }
}