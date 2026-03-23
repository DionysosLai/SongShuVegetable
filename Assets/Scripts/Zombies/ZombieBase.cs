using System;
using UnityEngine;
using SongShuVegetable.Core;
using SongShuVegetable.Data;
using SongShuVegetable.Plants;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Zombies
{
    public class ZombieBase : MonoBehaviour
    {
        // ── 事件 ─────────────────────────────────────────────
        public event Action<ZombieBase> OnDied;

        // ── Inspector 配置 ────────────────────────────────────
        [SerializeField] protected ZombieData _data;

        // ── 属性 ─────────────────────────────────────────────
        public ZombieData Data        => _data;
        public bool IsAlive           => _currentHp > 0;
        public bool IsSlowed          => _slowTimer > 0f;

        protected int   _currentHp;
        protected int   _armorHp;
        protected float _currentSpeed;
        protected float _slowTimer;
        protected float _attackTimer;
        protected PlantBase _targetPlant;
        protected bool _isAttacking;

        // 僵尸到达左侧边界时触发失败（X 坐标）
        private const float _defeatLineX = -2f;

        // ── 初始化 ────────────────────────────────────────────
        public virtual void Initialize(ZombieData data)
        {
            _data = data;
            _currentHp   = data.MaxHealth;
            _armorHp     = data.ArmorHealth;
            _currentSpeed = data.MoveSpeed;
            _slowTimer    = 0f;
            _attackTimer  = 0f;
            _isAttacking  = false;
            _targetPlant  = null;
            OnInitialized();
        }

        // ── 生命周期 ──────────────────────────────────────────
        private void Update()
        {
            if (!IsAlive || !GameManager.Instance.IsPlaying) return;

            UpdateSlow();

            if (_isAttacking)
                UpdateAttack();
            else
                UpdateMove();

            CheckDefeatLine();
        }

        // ── 受伤 ──────────────────────────────────────────────
        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            // 优先消耗护甲
            if (_armorHp > 0)
            {
                _armorHp -= damage;
                if (_armorHp <= 0)
                {
                    damage = -_armorHp;
                    _armorHp = 0;
                    OnArmorBroken();
                }
                else return;
            }

            _currentHp = Mathf.Max(0, _currentHp - damage);
            Log.Info("Zombie", $"{_data.DisplayName} 受到 {damage} 伤害，剩余 HP={_currentHp}");

            if (_currentHp <= 0)
                Die();
        }

        /// <summary>被冰豌豆命中，施加减速效果。</summary>
        public void ApplySlow(float multiplier, float duration)
        {
            _currentSpeed = _data.MoveSpeed * multiplier;
            _slowTimer = duration;
            Log.Info("Zombie", $"{_data.DisplayName} 被减速 {duration}s");
        }

        // ── 内部逻辑 ──────────────────────────────────────────
        private void UpdateMove()
        {
            // 检测正前方是否有植物
            var hit = Physics.OverlapSphere(
                transform.position + Vector3.left * 0.6f,
                0.4f,
                LayerMask.GetMask("Plant")
            );

            if (hit.Length > 0)
            {
                _targetPlant = hit[0].GetComponentInParent<PlantBase>();
                if (_targetPlant != null && _targetPlant.IsAlive)
                {
                    _isAttacking = true;
                    _attackTimer = 0f;
                    return;
                }
            }

            transform.Translate(Vector3.left * (_currentSpeed * Time.deltaTime));
        }

        private void UpdateAttack()
        {
            if (_targetPlant == null || !_targetPlant.IsAlive)
            {
                _isAttacking = false;
                _targetPlant = null;
                return;
            }

            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _data.AttackInterval)
            {
                _attackTimer = 0f;
                _targetPlant.TakeDamage(_data.AttackDamage);
                Log.Info("Zombie", $"{_data.DisplayName} 攻击 {_targetPlant.Data.DisplayName}，造成 {_data.AttackDamage} 伤害");
            }
        }

        private void UpdateSlow()
        {
            if (_slowTimer <= 0f) return;
            _slowTimer -= Time.deltaTime;
            if (_slowTimer <= 0f)
            {
                _slowTimer = 0f;
                _currentSpeed = _data.MoveSpeed;
                Log.Info("Zombie", $"{_data.DisplayName} 减速效果结束");
            }
        }

        private void CheckDefeatLine()
        {
            if (transform.position.x <= _defeatLineX)
            {
                Log.Warning("Zombie", $"{_data.DisplayName} 到达家园，触发失败");
                GameManager.Instance.TriggerDefeat();
            }
        }

        private void Die()
        {
            Log.Info("Zombie", $"{_data.DisplayName} 死亡");
            OnDie();
            OnDied?.Invoke(this);
            WaveManager.Instance.NotifyZombieDead(gameObject);

            if (_data.SunReward > 0)
                SunManager.Instance.AddSun(_data.SunReward);

            Destroy(gameObject, 0.1f);
        }

        // ── 子类钩子 ──────────────────────────────────────────
        protected virtual void OnInitialized() { }
        protected virtual void OnArmorBroken() { }
        protected virtual void OnDie() { }

        // ── 血量百分比 ────────────────────────────────────────
        public float HealthPercent =>
            _data != null && _data.MaxHealth > 0
                ? (float)_currentHp / _data.MaxHealth : 0f;
    }
}
