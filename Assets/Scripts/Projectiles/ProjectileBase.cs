using UnityEngine;
using SongShuVegetable.Zombies;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Projectiles
{
    /// <summary>
    /// 投射物基类：沿 +X 方向飞行，碰到僵尸造成伤害后回收。
    /// 子类重写 OnHitZombie() 添加额外效果（减速、爆炸等）。
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected int _damage = 20;
        [SerializeField] protected float _speed = 8f;
        [SerializeField] protected float _maxLifetime = 10f;

        private Rigidbody _rb;
        private float _lifeTimer;
        private GameObject _sourcePrefab;  // 归还对象池用

        // ── 生命周期 ──────────────────────────────────────────
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotation
                            | RigidbodyConstraints.FreezePositionY
                            | RigidbodyConstraints.FreezePositionZ;
        }

        private void OnEnable()
        {
            _lifeTimer = 0f;
            _rb.linearVelocity = Vector3.right * _speed;
        }

        private void Update()
        {
            _lifeTimer += Time.deltaTime;
            if (_lifeTimer >= _maxLifetime)
                Recycle();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Zombie")) return;

            var zombie = other.GetComponentInParent<ZombieBase>();
            if (zombie == null) return;

            zombie.TakeDamage(_damage);
            OnHitZombie(zombie);
            Recycle();
        }

        // ── 公开初始化 ────────────────────────────────────────
        public void Setup(int damage, float speed, GameObject sourcePrefab)
        {
            _damage = damage;
            _speed = speed;
            _sourcePrefab = sourcePrefab;
        }

        // ── 子类钩子 ──────────────────────────────────────────
        protected virtual void OnHitZombie(ZombieBase zombie) { }

        // ── 归还对象池 ────────────────────────────────────────
        protected void Recycle()
        {
            _rb.linearVelocity = Vector3.zero;
            ObjectPoolManager.Instance.Release(gameObject);
        }
    }
}
