using System;
using UnityEngine;
using SongShuVegetable.Core;
using SongShuVegetable.Data;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Plants
{
    /// <summary>
    /// 所有植物的抽象基类。
    /// 子类重写 OnInitialized() 做额外初始化，重写 OnDie() 做死亡特效。
    /// </summary>
    public abstract class PlantBase : MonoBehaviour
    {
        // ── 事件 ─────────────────────────────────────────────
        public event Action<PlantBase> OnPlantDied;

        // ── Inspector 配置 ────────────────────────────────────
        [SerializeField] protected PlantData _data;

        // ── 属性 ─────────────────────────────────────────────
        public PlantData Data => _data;
        public int CurrentHealth { get; private set; }
        public int MaxHealth     { get; private set; }
        public bool IsAlive      => CurrentHealth > 0;
        public int Row           { get; private set; }
        public int Col           { get; private set; }

        // ── 初始化（由 GridManager 种植后调用）────────────────
        public void Initialize(PlantData data, int row, int col)
        {
            _data = data;
            Row = row;
            Col = col;
            MaxHealth = data.MaxHealth;
            CurrentHealth = MaxHealth;
            OnInitialized();
            Log.Info("Plant", $"{data.DisplayName} 初始化于 ({row},{col})，HP={MaxHealth}");
        }

        // ── 受伤 ──────────────────────────────────────────────
        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            Log.Info("Plant", $"{_data.DisplayName} 受到 {damage} 伤害，剩余 HP={CurrentHealth}");

            if (CurrentHealth <= 0)
                Die();
        }

        // ── 死亡 ──────────────────────────────────────────────
        private void Die()
        {
            Log.Info("Plant", $"{_data.DisplayName} 死亡 ({Row},{Col})");
            OnDie();
            OnPlantDied?.Invoke(this);
            GridManager.Instance.RemovePlant(Row, Col);
            Destroy(gameObject, 0.1f);
        }

        // ── 血量百分比（UI 血条用）────────────────────────────
        public float HealthPercent =>
            MaxHealth > 0 ? (float)CurrentHealth / MaxHealth : 0f;

        // ── 子类钩子 ──────────────────────────────────────────
        protected virtual void OnInitialized() { }
        protected virtual void OnDie() { }
    }
}
