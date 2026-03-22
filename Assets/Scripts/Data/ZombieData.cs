using UnityEngine;

namespace SongShuVegetable.Data
{
    public enum ZombieType
    {
        BasicZombie,        // 普通僵尸
        ConeheadZombie,     // 路障僵尸
        BucketheadZombie,   // 铁桶僵尸
        FlagZombie          // 旗帜僵尸
    }

    [CreateAssetMenu(fileName = "ZombieData", menuName = "SongShu/Data/ZombieData")]
    public class ZombieData : ScriptableObject
    {
        [Header("基本信息")]
        public ZombieType ZombieType;
        public string DisplayName;

        [Header("生命值")]
        public int MaxHealth = 100;
        [Tooltip("护甲额外生命值（路障/铁桶掉落后失去护甲）")]
        public int ArmorHealth = 0;

        [Header("移动")]
        [Tooltip("正常移动速度（格/秒）")]
        public float MoveSpeed = 0.4f;
        [Tooltip("被冰豌豆命中后的减速比例，0.5 = 减速至 50%")]
        public float SlowMultiplier = 0.5f;
        [Tooltip("减速持续时间（秒）")]
        public float SlowDuration = 4f;

        [Header("攻击")]
        [Tooltip("每次攻击植物的伤害")]
        public int AttackDamage = 100;
        [Tooltip("攻击间隔（秒）")]
        public float AttackInterval = 1f;

        [Header("积分与奖励")]
        [Tooltip("击杀获得的阳光奖励")]
        public int SunReward = 0;

        [Header("旗帜僵尸专属")]
        [Tooltip("旗帜僵尸触发大波次时的额外僵尸数量")]
        public int FlagWaveBonus = 0;

        [Header("预制体")]
        public GameObject Prefab;
    }
}
