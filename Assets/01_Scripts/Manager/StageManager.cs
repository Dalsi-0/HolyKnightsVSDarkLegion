using Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs; // 몬스터 프리팹들
    public Transform[] spawnPoints; // 스폰 위치 배열
    private MonsterFactory monsterFactory;

    private IWaveState currentState;  // 현재 웨이브 상태
    public StageSO stageData;  
    private int currentWaveIndex = 0;  // 현재 웨이브 인덱스
    private float waveTimer = 0f;  // 대기 시간 관리
    private bool isWaveCleared = false;  // 몬스터가 전부 처치되었는지 체크

    void Start()
    {
        // 팩토리 초기화
        monsterFactory = new MonsterFactory(monsterPrefabs);

        // 처음엔 대기 상태로 시작
        ChangeState(new WaveWaitingState(this));
    }

    void Update()
    {
        currentState?.UpdateState();
    }

    public void ChangeState(IWaveState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }
    public void SpawnWave(string[] monsterIDs)
    {
        foreach (string monsterID in monsterIDs)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            MonsterStateMachine spawnedMonster = monsterFactory.SpawnMonster(monsterID, spawnPoint.position);
        }
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

    public void SetWaveCleared(bool cleared)
    {
        isWaveCleared = cleared;
        if (cleared)
        {
            ChangeState(new WaveEndState(this));
        }
    }

    public void SetStageData(int stageNumber)
    {
        stageData = DataManager.Instance.GetStageData(stageNumber);
    }
}
