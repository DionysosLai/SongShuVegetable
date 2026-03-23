using SongShuVegetable.Zombies;

namespace SongShuVegetable.Projectiles
{
    /// <summary>普通豌豆：命中后造成固定伤害，无额外效果。</summary>
    public class PeaProjectile : ProjectileBase
    {
        protected override void OnHitZombie(ZombieBase zombie) { }
    }
}
