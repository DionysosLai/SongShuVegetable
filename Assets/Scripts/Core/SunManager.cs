using System;
using UnityEngine;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Core
{
    public class SunManager : SingletonMonoBehaviour<SunManager>
    {
        // ── 事件 ─────────────────────────────────────────────
        public static event Action<int> OnSunChanged;   // 当前阳光总量

        // ── Inspector 配置 ────────────────────────────────────
        [SerializeField] private int _initialSun = 50;

        // ── 属性 ─────────────────────────────────────────────
        public int CurrentSun { get; private set; }

        // ── 生命周期 ──────────────────────────────────────────
        protected override void Awake()
        {
            base.Awake();
            CurrentSun = _initialSun;
        }

        private void Start()
        {
            OnSunChanged?.Invoke(CurrentSun);
        }

        // ── 公开接口 ──────────────────────────────────────────

        /// <summary>增加阳光（向日葵产出、点击阳光掉落等）。</summary>
        public void AddSun(int amount)
        {
            if (amount <= 0) return;
            CurrentSun += amount;
            OnSunChanged?.Invoke(CurrentSun);
            Log.Info("SunManager", $"+{amount} 阳光，当前 {CurrentSun}");
        }

        /// <summary>
        /// 消耗阳光（种植植物时调用）。
        /// 返回 true 表示消耗成功，false 表示阳光不足。
        /// </summary>
        public bool SpendSun(int cost)
        {
            if (cost <= 0) return true;
            if (CurrentSun < cost)
            {
                Log.Warning("SunManager", $"阳光不足：需要 {cost}，当前 {CurrentSun}");
                return false;
            }
            CurrentSun -= cost;
            OnSunChanged?.Invoke(CurrentSun);
            Log.Info("SunManager", $"-{cost} 阳光，当前 {CurrentSun}");
            return true;
        }

        /// <summary>判断是否负担得起某费用（不消耗）。</summary>
        public bool CanAfford(int cost) => CurrentSun >= cost;

        /// <summary>关卡开始时用 LevelData 的初始阳光重置。</summary>
        public void Initialize(int initialSun)
        {
            _initialSun = initialSun;
            CurrentSun = initialSun;
            OnSunChanged?.Invoke(CurrentSun);
            Log.Info("SunManager", $"初始阳光设置为 {initialSun}");
        }
    }
}
