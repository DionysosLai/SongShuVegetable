using System;
using UnityEngine;
using SongShuVegetable.Data;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Core
{
    /// <summary>
    /// 草坪格子管理器。
    /// 坐标系：Row 为 Z 轴方向（0=最上行），Col 为 X 轴方向（0=最左列）。
    /// 世界坐标由 GridOrigin + CellSize 推算。
    /// </summary>
    public class GridManager : SingletonMonoBehaviour<GridManager>
    {
        // ── 事件 ─────────────────────────────────────────────
        public static event Action<int, int, GameObject> OnPlantPlaced;    // row, col, plantObj
        public static event Action<int, int> OnPlantRemoved;               // row, col

        // ── Inspector 配置 ────────────────────────────────────
        [SerializeField] private int _rows = 5;
        [SerializeField] private int _cols = 9;
        [SerializeField] private float _cellSize = 1.2f;
        [Tooltip("格子左上角的世界坐标原点")]
        [SerializeField] private Vector3 _gridOrigin = Vector3.zero;

        // ── 属性 ─────────────────────────────────────────────
        public int Rows => _rows;
        public int Cols => _cols;
        public float CellSize => _cellSize;

        private GameObject[,] _plantGrid;

        // ── 生命周期 ──────────────────────────────────────────
        protected override void Awake()
        {
            base.Awake();
            _plantGrid = new GameObject[_rows, _cols];
            Log.Info("GridManager", $"草坪初始化 {_rows}×{_cols}，格子大小 {_cellSize}");
        }

        // ── 公开接口 ──────────────────────────────────────────

        /// <summary>在指定格子种植，返回是否成功。</summary>
        public bool PlacePlant(int row, int col, GameObject plantPrefab)
        {
            if (!IsValidCell(row, col))
            {
                Log.Warning("GridManager", $"坐标越界 ({row},{col})");
                return false;
            }
            if (_plantGrid[row, col] != null)
            {
                Log.Warning("GridManager", $"({row},{col}) 已有植物");
                return false;
            }

            Vector3 pos = GetWorldPosition(row, col);
            var plant = Instantiate(plantPrefab, pos, Quaternion.identity);
            _plantGrid[row, col] = plant;

            OnPlantPlaced?.Invoke(row, col, plant);
            Log.Info("GridManager", $"种植 {plantPrefab.name} 于 ({row},{col})");
            return true;
        }

        /// <summary>移除指定格子的植物。</summary>
        public void RemovePlant(int row, int col)
        {
            if (!IsValidCell(row, col) || _plantGrid[row, col] == null) return;

            Destroy(_plantGrid[row, col]);
            _plantGrid[row, col] = null;
            OnPlantRemoved?.Invoke(row, col);
            Log.Info("GridManager", $"移除植物 ({row},{col})");
        }

        /// <summary>获取格子上的植物 GameObject，无则返回 null。</summary>
        public GameObject GetPlant(int row, int col)
            => IsValidCell(row, col) ? _plantGrid[row, col] : null;

        /// <summary>格子是否为空（可种植）。</summary>
        public bool IsCellEmpty(int row, int col)
            => IsValidCell(row, col) && _plantGrid[row, col] == null;

        /// <summary>世界坐标转格子坐标，返回是否在范围内。</summary>
        public bool WorldToGrid(Vector3 worldPos, out int row, out int col)
        {
            Vector3 local = worldPos - _gridOrigin;
            col = Mathf.FloorToInt(local.x / _cellSize);
            row = Mathf.FloorToInt(-local.z / _cellSize);
            return IsValidCell(row, col);
        }

        /// <summary>格子坐标转世界坐标（格子中心）。</summary>
        public Vector3 GetWorldPosition(int row, int col)
            => _gridOrigin + new Vector3(col * _cellSize + _cellSize * 0.5f, 0f,
                                         -row * _cellSize - _cellSize * 0.5f);

        /// <summary>获取指定行所有僵尸行进方向上第一个有植物的格子 X 世界坐标。</summary>
        public float GetFirstPlantXInRow(int row, float zombieX)
        {
            for (int col = _cols - 1; col >= 0; col--)
            {
                if (_plantGrid[row, col] != null)
                {
                    float plantX = GetWorldPosition(row, col).x;
                    if (plantX < zombieX) return plantX;
                }
            }
            return float.MinValue;
        }

        // ── 内部工具 ──────────────────────────────────────────
        private bool IsValidCell(int row, int col)
            => row >= 0 && row < _rows && col >= 0 && col < _cols;

        // ── Gizmos 调试可视化 ─────────────────────────────────
        private void OnDrawGizmos()
        {
            if (_rows <= 0 || _cols <= 0) return;
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.4f);
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    Vector3 center = _gridOrigin + new Vector3(c * _cellSize + _cellSize * 0.5f,
                                                               0f,
                                                               -r * _cellSize - _cellSize * 0.5f);
                    Gizmos.DrawWireCube(center, new Vector3(_cellSize * 0.95f, 0.05f, _cellSize * 0.95f));
                }
            }
        }
    }
}
