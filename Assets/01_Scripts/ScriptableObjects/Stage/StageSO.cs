using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveData
{
    public int waveNumber;
    public string[] monsterIDs;
    public int[] monsterCounts;
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public class StageSO : ScriptableObject
{
    [SerializeField] private int stageNumber;
    [SerializeField] private List<WaveData> waves = new List<WaveData>();

    // Getter
    public int StageNumber => stageNumber;
    public List<WaveData> Waves => waves;

    public void SetData(int stageNumber, List<WaveData> waves)
    {
        this.stageNumber = stageNumber;
        this.waves = waves;
    }
}