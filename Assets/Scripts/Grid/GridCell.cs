using UnityEngine;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Grid
{
    public enum CellState
    {
        Empty,      // 空格，可种植
        Occupied,   // 已有植物
        Blocked     // 不可种植（道路、水池等）
    }

    /// <summary>
    /// 挂载在每个草坪格子 GameObject 上。
    /// 负责视觉高亮反馈（悬停/可种植/不可种植）和状态管理。
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        // ── Inspector 配置 ────────────────────────────────────
        [SerializeField] private int _row;
        [SerializeField] private int _col;
        [SerializeField] private Renderer _cellRenderer;

        [Header("高亮颜色")]
        [SerializeField] private Color _normalColor    = new Color(0.4f, 0.8f, 0.2f, 0.3f);
        [SerializeField] private Color _hoverColor     = new Color(1f,   1f,   0.3f, 0.5f);
        [SerializeField] private Color _occupiedColor  = new Color(0.8f, 0.2f, 0.2f, 0.4f);
        [SerializeField] private Color _blockedColor   = new Color(0.3f, 0.3f, 0.3f, 0.4f);

        // ── 属性 ─────────────────────────────────────────────
        public int Row => _row;
        public int Col => _col;
        public CellState State { get; private set; } = CellState.Empty;
        public bool IsEmpty => State == CellState.Empty;

        private Material _mat;
        private static readonly int _colorPropID = Shader.PropertyToID("_BaseColor");

        // ── 生命周期 ──────────────────────────────────────────
        private void Awake()
        {
            if (_cellRenderer == null)
                _cellRenderer = GetComponent<Renderer>();

            if (_cellRenderer != null)
            {
                _mat = _cellRenderer.material;
                SetColor(_normalColor);
            }
        }

        // ── 公开接口 ──────────────────────────────────────────

        public void Initialize(int row, int col)
        {
            _row = row;
            _col = col;
        }

        public void SetState(CellState state)
        {
            State = state;
            RefreshColor();
        }

        // ── 鼠标悬停高亮 ──────────────────────────────────────
        private void OnMouseEnter()
        {
            if (State == CellState.Empty)
                SetColor(_hoverColor);
        }

        private void OnMouseExit()
        {
            RefreshColor();
        }

        private void OnMouseDown()
        {
            if (State == CellState.Blocked) return;
            PlantSelector.Instance?.TryPlantAt(_row, _col);
        }

        // ── 内部工具 ──────────────────────────────────────────
        private void RefreshColor()
        {
            switch (State)
            {
                case CellState.Empty:    SetColor(_normalColor);   break;
                case CellState.Occupied: SetColor(_occupiedColor); break;
                case CellState.Blocked:  SetColor(_blockedColor);  break;
            }
        }

        private void SetColor(Color color)
        {
            if (_mat == null) return;
            if (_mat.HasProperty(_colorPropID))
                _mat.SetColor(_colorPropID, color);
            else
                _mat.color = color;
        }
    }
}
