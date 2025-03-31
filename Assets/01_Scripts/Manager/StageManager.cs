using Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private GameObject[] monsterPrefabs; // 몬스터 프리팹들
    [SerializeField] private Transform[] spawnPoints; // 스폰 위치 배열
    private MonsterFactory monsterFactory;
    private IWaveState currentState;  // 현재 웨이브 상태
    private StageSO stageData;  
    private int currentWaveIndex = 0;  // 현재 웨이브 인덱스

    void Start()
    {
        // 팩토리 초기화
        monsterFactory = new MonsterFactory(monsterPrefabs);

        SetStageData(1);

        // 처음엔 대기 상태로 시작
        // ChangeState(new WaveWaitingState(this));
    }

    void Update()
    {
        if(currentState != null)
        {
            currentState?.UpdateState();
        }
    }

    public void ChangeState(IWaveState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    public void StartNextWave()
    {
        if (currentWaveIndex < stageData.Waves.Count)
        {
            ChangeState(new WaveSpawningState(this, stageData.Waves[currentWaveIndex]));
            currentWaveIndex++;
        }
        else
        {
            Debug.Log("스테이지 클리어");
        }
    }

    public void SetWaveCleared()
    {
        ChangeState(new WaveEndState(this));
    }

    public void SetStageData(int stageNumber)
    {
        stageData = DataManager.Instance.GetStageData(stageNumber);
    }

    public MonsterFactory GetMonsterFactory()
    {
        return monsterFactory;
    }

    public Transform[] GetSpawnPoints()
    {
        return spawnPoints;
    }
}
