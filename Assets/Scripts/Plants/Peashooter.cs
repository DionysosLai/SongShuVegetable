using UnityEngine;
using SongShuVegetable.Core;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Plants
{
    /// <summary>
    /// 豌豆射手：同行有僵尸时才开始计时射击。
    /// </summary>
    public class Peashooter : PlantBase
    {
        private float _shootTimer = 0f;
        private bool _isShooting = false;

        private void Update()
        {
            if (!IsAlive || !GameManager.Instance.IsPlaying) return;

            bool zombieInRow = ZombieInRow();

            if (zombieInRow)
            {
                _isShooting = true;
                _shootTimer += Time.deltaTime;
                if (_shootTimer >= _data.AttackInterval)
                {
                    _shootTimer = 0f;
                    Shoot();
                }
            }
            else
            {
                _isShooting = false;
                _shootTimer = 0f;
            }
        }

        protected override void OnInitialized()
        {
            _shootTimer = 0f;
        }

        private void Shoot()
        {
            if (_data.ProjectilePrefab == null)
            {
                Log.Warning("Peashooter", "未设置 ProjectilePrefab");
                return;
            }

            var spawnPos = transform.position + Vector3.right * 0.5f;
            ObjectPoolManager.Instance.Get(_data.ProjectilePrefab, spawnPos);
            Log.Info("Peashooter", $"({Row},{Col}) 发射豌豆");
        }

        private bool ZombieInRow()
        {
            // 在同行 X 轴正方向（右侧）检测是否有僵尸
            var hits = Physics.RaycastAll(
                transform.position,
                Vector3.right,
                100f
            );

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Zombie"))
                    return true;
            }
            return false;
        }
    }
}
