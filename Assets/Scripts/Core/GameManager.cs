using System;
using UnityEngine;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Core
{
    public enum GameState
    {
        Preparing,  // 准备阶段（选牌、倒计时）
        Playing,    // 游戏进行中
        Paused,     // 暂停
        Victory,    // 胜利
        Defeat      // 失败
    }

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        // ── 事件 ─────────────────────────────────────────────
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameStarted;
        public static event Action OnGameVictory;
        public static event Action OnGameDefeat;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;

        // ── 属性 ─────────────────────────────────────────────
        public GameState CurrentState { get; private set; } = GameState.Preparing;
        public bool IsPlaying => CurrentState == GameState.Playing;

        // ── 生命周期 ──────────────────────────────────────────
        protected override void Awake()
        {
            base.Awake();
            Log.Info("GameManager", "初始化");
        }

        private void Start()
        {
            SetState(GameState.Preparing);
        }

        // ── 公开接口 ──────────────────────────────────────────
        public void StartGame()
        {
            if (CurrentState != GameState.Preparing)
            {
                Log.Warning("GameManager", $"StartGame 调用时状态为 {CurrentState}，忽略");
                return;
            }
            SetState(GameState.Playing);
            OnGameStarted?.Invoke();
            Log.Info("GameManager", "游戏开始");
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            Time.timeScale = 0f;
            SetState(GameState.Paused);
            OnGamePaused?.Invoke();
            Log.Info("GameManager", "游戏暂停");
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            Time.timeScale = 1f;
            SetState(GameState.Playing);
            OnGameResumed?.Invoke();
            Log.Info("GameManager", "游戏恢复");
        }

        public void TriggerVictory()
        {
            if (CurrentState != GameState.Playing) return;
            SetState(GameState.Victory);
            OnGameVictory?.Invoke();
            Log.Info("GameManager", "胜利！");
        }

        public void TriggerDefeat()
        {
            if (CurrentState != GameState.Playing) return;
            SetState(GameState.Defeat);
            OnGameDefeat?.Invoke();
            Log.Info("GameManager", "失败！");
        }

        // ── 内部方法 ──────────────────────────────────────────
        private void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            Log.Info("GameManager", $"状态切换：{CurrentState} → {newState}");
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }
}
