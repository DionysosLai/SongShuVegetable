using SongShuVegetable.Data;

namespace SongShuVegetable.Zombies
{
    /// <summary>普通僵尸：使用 ZombieData 基础属性，无额外逻辑。</summary>
    public class BasicZombie : ZombieBase
    {
        private void Start()
        {
            if (_data != null)
                Initialize(_data);
        }
    }
}
