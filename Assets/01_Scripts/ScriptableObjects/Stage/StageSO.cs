using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public class StageSO : ScriptableObject
{
    [SerializeField] private int stageNumber;
    [SerializeField] private int stageWave;
    [SerializeField] private string[] monsterIDs;
    [SerializeField] private int monsterCount;
    [SerializeField] private float spawnInterval;

    // Getter
    public int StageNumber => stageNumber;
    public int StageWave => stageWave;
    public string[] MonsterIDs => monsterIDs;
    public int MonsterCount => monsterCount;
    public float SpawnInterval => spawnInterval;

    public void SetData(int stageNumber, int stageWave, string[] monsterIDs, int monsterCount, float spawnInterval)
    {
        this.stageNumber = stageNumber;
        this.stageWave = stageWave;
        this.monsterIDs = monsterIDs;
        this.monsterCount = monsterCount;
        this.spawnInterval = spawnInterval;
    }
}