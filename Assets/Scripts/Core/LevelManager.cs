using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SongShuVegetable.Data;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Core
{
    public class LevelManager : SingletonMonoBehaviour<LevelManager>
    {
        // ── 事件 ─────────────────────────────────────────────
        public static event Action<LevelData> OnLevelLoaded;
        public static event Action OnLevelRestarted;

        // ── Inspector 配置 ────────────────────────────────────
        [Tooltip("所有关卡数据，按顺序排列")]
        [SerializeField] private List<LevelData> _levels = new();
        [Tooltip("游戏场景名称")]
        [SerializeField] private string _gameplaySceneName = "GamePlay";
        [Tooltip("主菜单场景名称")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        // ── 属性 ─────────────────────────────────────────────
        public LevelData CurrentLevel { get; private set; }
        public int CurrentLevelIndex { get; private set; } = 0;
        public bool HasNextLevel => CurrentLevelIndex + 1 < _levels.Count;

        // ── 生命周期 ──────────────────────────────────────────
        protected override void Awake()
        {
            base.Awake();
            GameManager.OnGameVictory += OnVictory;
            GameManager.OnGameDefeat  += OnDefeat;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.OnGameVictory -= OnVictory;
            GameManager.OnGameDefeat  -= OnDefeat;
        }

        // ── 公开接口 ──────────────────────────────────────────

        /// <summary>加载指定索引的关卡并初始化所有管理器。</summary>
        public void LoadLevel(int index)
        {
            if (index < 0 || index >= _levels.Count)
            {
                Log.Error("LevelManager", $"关卡索引越界：{index}（共 {_levels.Count} 关）");
                return;
            }

            CurrentLevelIndex = index;
            CurrentLevel = _levels[index];
            Log.Info("LevelManager", $"加载关卡 [{CurrentLevel.LevelName}]");

            StartCoroutine(LoadLevelRoutine());
        }

        public void LoadNextLevel()
        {
            if (!HasNextLevel)
            {
                Log.Warning("LevelManager", "已是最后一关，返回主菜单");
                LoadMainMenu();
                return;
            }
            LoadLevel(CurrentLevelIndex + 1);
        }

        public void RestartCurrentLevel()
        {
            Log.Info("LevelManager", $"重玩关卡 [{CurrentLevel?.LevelName}]");
            OnLevelRestarted?.Invoke();
            LoadLevel(CurrentLevelIndex);
        }

        public void LoadMainMenu()
        {
            Log.Info("LevelManager", "返回主菜单");
            Time.timeScale = 1f;
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        // ── 内部流程 ──────────────────────────────────────────
        private IEnumerator LoadLevelRoutine()
        {
            Time.timeScale = 1f;

            // 异步加载游戏场景（已在场景则直接重载）
            var op = SceneManager.LoadSceneAsync(_gameplaySceneName);
            yield return op;

            // 场景加载完毕后初始化各管理器
            SunManager.Instance.Initialize(CurrentLevel.InitialSun);
            WaveManager.Instance.Initialize(CurrentLevel);

            OnLevelLoaded?.Invoke(CurrentLevel);
            Log.Info("LevelManager", $"关卡 [{CurrentLevel.LevelName}] 初始化完毕");

            GameManager.Instance.StartGame();
            WaveManager.Instance.StartWaves();
        }

        private void OnVictory()
        {
            Log.Info("LevelManager", "胜利处理：等待玩家操作");
        }

        private void OnDefeat()
        {
            Log.Info("LevelManager", "失败处理：等待玩家操作");
        }
    }
}
