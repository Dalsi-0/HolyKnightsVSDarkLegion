using UnityEngine;

public class WaveSpawningState : IWaveState
{
    private StageManager stageManager;
    private WaveData waveData;
    private float spawnTimer = 0f;
    private float spawnVariance = 0.5f;
    private int totalSpawnedMonsters = 0;
    private int deadMonsters = 0;


    public WaveSpawningState(StageManager stageManager, WaveData waveData)
    {
        this.stageManager = stageManager;
        this.waveData = waveData.Clone(); // 복사본을 생성하여 사용
        this.stageManager.SetWaveSpawningState(this);
    }

    public void EnterState()
    {
        totalSpawnedMonsters = 0;
        deadMonsters = 0;
    }

    public void UpdateState()
    {
        spawnTimer += Time.deltaTime;

        // spawnInterval에 오차 범위를 추가하여 랜덤한 시간에 소환
        float spawnIntervalWithVariance = waveData.spawnInterval + Random.Range(-spawnVariance, spawnVariance);

        // 일정 시간 간격마다 몬스터를 하나 소환
        if (spawnTimer >= spawnIntervalWithVariance && !AllMonstersSpawned())
        {
            SpawnMonster();
            spawnTimer = 0f;
        }
    }

    private void SpawnMonster()
    {
        // 남은 몬스터 중에서 하나를 랜덤하게 소환
        int randomIndex = Random.Range(0, waveData.monsterIDs.Length);

        // 해당 몬스터가 소환 가능한지 체크 (카운트가 0이 아니어야 함)
        while (waveData.monsterCounts[randomIndex] <= 0)
        {
            // 소환이 불가능한 몬스터라면 다른 몬스터를 랜덤하게 선택
            randomIndex = Random.Range(0, waveData.monsterIDs.Length);
        }

        // 해당 몬스터 소환
        string monsterID = waveData.monsterIDs[randomIndex];
        int randomSpawnIndex = Random.Range(0, stageManager.GetSpawnPoints().Length);
        Transform spawnPoint = stageManager.GetSpawnPoints()[randomSpawnIndex];

        MonsterFactory monsterFactory = stageManager.GetMonsterFactory();
        monsterFactory.SpawnMonster(monsterID, spawnPoint.position, GameManager.Instance);
        totalSpawnedMonsters++;

        // 소환된 몬스터의 카운트를 1 감소
        waveData.monsterCounts[randomIndex]--;
    }

    public void OnMonsterDied()
    {
        deadMonsters++;

        if (deadMonsters >= totalSpawnedMonsters)
        {
            StageManager.Instance.SetWaveCleared(); // 모든 몬스터가 죽으면 웨이브 종료
        }
    }

    // 모든 몬스터가 소환되었는지 확인하는 함수
    private bool AllMonstersSpawned()
    {
        foreach (int count in waveData.monsterCounts)
        {
            if (count > 0)
            {
                return false; 
            }
        }
        return true; 
    }


}

