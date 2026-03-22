using System.Collections.Generic;
using UnityEngine;

namespace SongShuVegetable.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "SongShu/Data/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("关卡信息")]
        public string LevelName;
        public int LevelIndex;
        [TextArea(2, 3)]
        public string Description;

        [Header("草坪配置")]
        [Tooltip("草坪行数")]
        public int Rows = 5;
        [Tooltip("草坪列数")]
        public int Columns = 9;

        [Header("初始资源")]
        public int InitialSun = 50;

        [Header("可用植物")]
        [Tooltip("本关卡可选的植物数据列表")]
        public List<PlantData> AvailablePlants = new();

        [Header("波次列表")]
        [Tooltip("按顺序排列的所有波次，最后一波打完即胜利")]
        public List<WaveData> Waves = new();

        [Header("音乐")]
        public AudioClip BGM;

        public int TotalWaves => Waves.Count;
    }
}
