using System;
using System.Collections.Generic;
using UnityEngine;

namespace SongShuVegetable.Data
{
    /// <summary>
    /// 一条僵尸生成指令：在哪条路（Row）、延迟多久、生成哪种僵尸。
    /// </summary>
    [Serializable]
    public class ZombieSpawnEntry
    {
        [Tooltip("草坪行索引（0 = 最上行）")]
        public int Row = 0;

        [Tooltip("本波开始后延迟多少秒生成")]
        public float SpawnDelay = 0f;

        public ZombieData ZombieData;
    }

    /// <summary>
    /// 单波次配置数据（内嵌在 LevelData 里，不单独作为 SO 文件）。
    /// </summary>
    [Serializable]
    public class WaveData
    {
        [Tooltip("本波僵尸全部生成完毕后，等待多少秒才触发下一波")]
        public float WaveInterval = 20f;

        [Tooltip("是否为大波次（显示大波横幅）")]
        public bool IsBigWave = false;

        public List<ZombieSpawnEntry> Zombies = new();
    }
}
