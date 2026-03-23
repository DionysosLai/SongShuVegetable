using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SongShuVegetable.Data;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Core
{
    public class WaveManager : SingletonMonoBehaviour<WaveManager>
    {
        // ── 事件 ─────────────────────────────────────────────
        public static event Action<int, int> OnWaveStarted;    // 当前波次索引, 总波次
        public static event Action<int, bool> OnWaveCleared;   // 波次索引, 是否大波次
        public static event Action OnAllWavesCleared;
        public static event Action<GameObject> OnZombieSpawned;

        // ── Inspector 配置 ────────────────────────────────────
        [Tooltip("僵尸生成线的 X 世界坐标（草坪右侧屏幕外）")]
        [SerializeField] private float _spawnX = 14f;
        [Tooltip("每行对应的 Z 世界坐标，由 GridManager 行数决定，留空则自动计算")]
        [SerializeField] private float[] _rowZPositions;

        // ── 属性 ─────────────────────────────────────────────
        public int CurrentWaveIndex { get; private set; } = -1;
        public int TotalWaves { get; private set; }
        public int AliveZombieCount { get; private set; }
        public bool IsRunning { get; private set; }

        private LevelData _levelData;
        private readonly List<GameObject> _aliveZombies = new();

        // ── 公开接口 ──────────────────────────────────────────

        public void Initialize(LevelData levelData)
        {
            _levelData = levelData;
            TotalWaves = levelData.TotalWaves;
            CurrentWaveIndex = -1;
            IsRunning = false;
            _aliveZombies.Clear();
            AliveZombieCount = 0;

            AutoFillRowPositions();
            Log.Info("WaveManager", $"初始化关卡 [{levelData.LevelName}]，共 {TotalWaves} 波");
        }

        public void StartWaves()
        {
            if (_levelData == null)
            {
                Log.Error("WaveManager", "未初始化 LevelData，无法开始波次");
                return;
            }
            IsRunning = true;
            StartCoroutine(WaveRoutine());
        }

        public void StopWaves()
        {
            IsRunning = false;
            StopAllCoroutines();
        }

        /// <summary>僵尸死亡时调用，减少存活计数。</summary>
        public void NotifyZombieDead(GameObject zombie)
        {
            if (_aliveZombies.Remove(zombie))
            {
                AliveZombieCount = _aliveZombies.Count;
                Log.Info("WaveManager", $"僵尸死亡，剩余 {AliveZombieCount}");
            }
        }

        // ── 波次协程 ──────────────────────────────────────────
        private IEnumerator WaveRoutine()
        {
            for (int i = 0; i < TotalWaves; i++)
            {
                if (!IsRunning) yield break;

                CurrentWaveIndex = i;
                var wave = _levelData.Waves[i];
                OnWaveStarted?.Invoke(i, TotalWaves);
                Log.Info("WaveManager", $"第 {i + 1}/{TotalWaves} 波开始{(wave.IsBigWave ? "【大波次】" : "")}");

                yield return StartCoroutine(SpawnWaveRoutine(wave));

                // 等待本波所有僵尸死亡
                yield return new WaitUntil(() => AliveZombieCount == 0);

                OnWaveCleared?.Invoke(i, wave.IsBigWave);
                Log.Info("WaveManager", $"第 {i + 1} 波清空");

                if (i < TotalWaves - 1)
                    yield return new WaitForSeconds(wave.WaveInterval);
            }

            OnAllWavesCleared?.Invoke();
            Log.Info("WaveManager", "所有波次已清空，触发胜利");
            GameManager.Instance.TriggerVictory();
        }

        private IEnumerator SpawnWaveRoutine(WaveData wave)
        {
            foreach (var entry in wave.Zombies)
            {
                if (entry.SpawnDelay > 0f)
                    yield return new WaitForSeconds(entry.SpawnDelay);

                SpawnZombie(entry);
            }
        }

        // ── 僵尸生成 ──────────────────────────────────────────
        private void SpawnZombie(ZombieSpawnEntry entry)
        {
            if (entry.ZombieData == null || entry.ZombieData.Prefab == null)
            {
                Log.Warning("WaveManager", "ZombieSpawnEntry 缺少 ZombieData 或 Prefab");
                return;
            }

            float z = GetRowZ(entry.Row);
            var pos = new Vector3(_spawnX, 0f, z);
            var zombie = Instantiate(entry.ZombieData.Prefab, pos, Quaternion.identity);

            _aliveZombies.Add(zombie);
            AliveZombieCount = _aliveZombies.Count;
            OnZombieSpawned?.Invoke(zombie);
            Log.Info("WaveManager", $"生成 {entry.ZombieData.DisplayName} 于行 {entry.Row}");
        }

        // ── 工具 ─────────────────────────────────────────────
        private float GetRowZ(int row)
        {
            if (_rowZPositions != null && row < _rowZPositions.Length)
                return _rowZPositions[row];

            // 回退：按 GridManager 格子大小自动推算
            if (GridManager.Instance != null)
                return GridManager.Instance.GetWorldPosition(row, 0).z;

            return -row * 1.2f;
        }

        private void AutoFillRowPositions()
        {
            if (_rowZPositions != null && _rowZPositions.Length > 0) return;
            if (GridManager.Instance == null) return;

            int rows = GridManager.Instance.Rows;
            _rowZPositions = new float[rows];
            for (int r = 0; r < rows; r++)
                _rowZPositions[r] = GridManager.Instance.GetWorldPosition(r, 0).z;
        }
    }
}
