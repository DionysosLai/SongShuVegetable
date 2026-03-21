using UnityEngine;

namespace SongShuVegetable.Data
{
    public enum PlantType
    {
        Peashooter,     // 豌豆射手
        Sunflower,      // 向日葵
        WallNut,        // 坚果墙
        SnowPea,        // 寒冰射手
        CherryBomb,     // 樱桃炸弹
        PotatoMine      // 土豆雷
    }

    [CreateAssetMenu(fileName = "PlantData", menuName = "SongShu/Data/PlantData")]
    public class PlantData : ScriptableObject
    {
        [Header("基本信息")]
        public PlantType PlantType;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public Sprite CardIcon;

        [Header("费用与冷却")]
        public int SunCost = 100;
        [Tooltip("两次种植之间的冷却时间（秒）")]
        public float Cooldown = 7.5f;

        [Header("生命值")]
        public int MaxHealth = 100;

        [Header("攻击属性（射手类植物）")]
        public bool CanAttack = false;
        public float AttackInterval = 1.4f;
        public int AttackDamage = 20;
        public float ProjectileSpeed = 8f;
        public GameObject ProjectilePrefab;

        [Header("向日葵专属")]
        public bool ProducesSun = false;
        public int SunProduceAmount = 25;
        public float SunProduceInterval = 24f;
        public GameObject SunPrefab;

        [Header("爆炸类植物（樱桃炸弹 / 土豆雷）")]
        public bool IsExplosive = false;
        public float ExplosionRadius = 1.5f;
        public int ExplosionDamage = 1800;
        [Tooltip("激活后延迟爆炸时间（秒），0 表示立即爆炸")]
        public float ExplosionDelay = 0f;
        [Tooltip("土豆雷：埋地武装时间（秒），武装前不触发爆炸）")]
        public float ArmDelay = 14f;

        [Header("预制体")]
        public GameObject Prefab;
    }
}
