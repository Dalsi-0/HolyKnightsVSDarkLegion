using UnityEngine;

public class WaveSpawningState : IWaveState
{
    private StageManager stageManager;
    private WaveData waveData;
    private int spawnedCount = 0;
    private float spawnTimer = 0f;

    public WaveSpawningState(StageManager stageManager, WaveData waveData)
    {
        this.stageManager = stageManager;
        this.waveData = waveData;
    }

    public void EnterState()
    {
        Debug.Log("웨이브 시작 몬스터 소환 중");
    }

    public void UpdateState()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= waveData.spawnInterval && spawnedCount < waveData.monsterIDs.Length)
        {
           // MonsterFactory.SpawnMonster(waveData.monsterIDs[spawnedCount]);
            spawnedCount++;
            spawnTimer = 0f;
        }

        if (spawnedCount >= waveData.monsterIDs.Length)
        {
            stageManager.SetWaveCleared(true); // 모든 몬스터가 소환되었음
        }
    }
}
