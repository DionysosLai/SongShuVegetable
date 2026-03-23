using System.Collections.Generic;
using UnityEngine;
using SongShuVegetable.Core;
using SongShuVegetable.Data;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Grid
{
    /// <summary>
    /// 玩家选牌与种植交互控制器（Singleton）。
    /// 流程：点击植物卡片 → 选中 → 点击格子 → 扣阳光 → 种植 → 进入冷却。
    /// </summary>
    public class PlantSelector : SingletonMonoBehaviour<PlantSelector>
    {
        // ── 属性 ─────────────────────────────────────────────
        public PlantData SelectedPlant { get; private set; }
        public bool HasSelection => SelectedPlant != null;

        // 每种植物的冷却剩余时间（秒）
        private readonly Dictionary<PlantData, float> _cooldowns = new();

        // ── 生命周期 ──────────────────────────────────────────
        private void Update()
        {
            TickCooldowns();

            // 右键 / Escape 取消选择
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                CancelSelection();
        }

        // ── 公开接口 ──────────────────────────────────────────

        /// <summary>UI 卡片点击时调用。</summary>
        public void SelectPlant(PlantData data)
        {
            if (data == null) return;

            if (!SunManager.Instance.CanAfford(data.SunCost))
            {
                Log.Warning("PlantSelector", $"阳光不足，无法选择 {data.DisplayName}（需 {data.SunCost}）");
                return;
            }

            if (IsOnCooldown(data))
            {
                Log.Warning("PlantSelector", $"{data.DisplayName} 冷却中（剩余 {GetCooldown(data):F1}s）");
                return;
            }

            SelectedPlant = data;
            Log.Info("PlantSelector", $"选中：{data.DisplayName}");
        }

        public void CancelSelection()
        {
            if (SelectedPlant == null) return;
            Log.Info("PlantSelector", "取消选择");
            SelectedPlant = null;
        }

        /// <summary>GridCell.OnMouseDown 调用，尝试在指定格子种植。</summary>
        public void TryPlantAt(int row, int col)
        {
            if (!GameManager.Instance.IsPlaying)
            {
                Log.Warning("PlantSelector", "游戏未开始，无法种植");
                return;
            }

            if (SelectedPlant == null)
            {
                Log.Warning("PlantSelector", "未选择植物");
                return;
            }

            if (!GridManager.Instance.IsCellEmpty(row, col))
            {
                Log.Warning("PlantSelector", $"({row},{col}) 已有植物");
                return;
            }

            if (!SunManager.Instance.SpendSun(SelectedPlant.SunCost))
                return;

            bool placed = GridManager.Instance.PlacePlant(row, col, SelectedPlant.Prefab);
            if (placed)
            {
                StartCooldown(SelectedPlant);
                Log.Info("PlantSelector", $"种植 {SelectedPlant.DisplayName} 于 ({row},{col})");
            }
            else
            {
                // 种植失败，退还阳光
                SunManager.Instance.AddSun(SelectedPlant.SunCost);
                Log.Warning("PlantSelector", "种植失败，阳光已退还");
            }

            SelectedPlant = null;
        }

        // ── 冷却系统 ──────────────────────────────────────────
        public bool IsOnCooldown(PlantData data)
            => _cooldowns.TryGetValue(data, out float t) && t > 0f;

        public float GetCooldown(PlantData data)
            => _cooldowns.TryGetValue(data, out float t) ? Mathf.Max(0f, t) : 0f;

        private void StartCooldown(PlantData data)
        {
            _cooldowns[data] = data.Cooldown;
        }

        private void TickCooldowns()
        {
            if (!GameManager.Instance.IsPlaying) return;

            var keys = new List<PlantData>(_cooldowns.Keys);
            foreach (var key in keys)
            {
                if (_cooldowns[key] > 0f)
                    _cooldowns[key] -= Time.deltaTime;
            }
        }
    }
}
