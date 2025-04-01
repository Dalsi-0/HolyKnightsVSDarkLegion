using Monsters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private GameObject[] monsterPrefabs; // 몬스터 프리팹들
    [SerializeField] private Transform[] spawnPoints; // 스폰 위치 배열
    [SerializeField] private GameObject spawnParticle; // 스폰 파티클
    [SerializeField] private Animator stageUIAnim;
    [SerializeField] private Transform[] levels;
    [SerializeField] private TextMeshProUGUI waveText;

    private MonsterFactory monsterFactory;
    private IWaveState currentState;  // 현재 웨이브 상태
    private StageSO stageData;  
    private int currentWaveIndex = 0;  // 현재 웨이브 인덱스
    private WaveSpawningState waveSpawningState;


    void Start()
    {
        stageUIAnim.Play("FadeIn");

        // 팩토리 초기화
        monsterFactory = new MonsterFactory(monsterPrefabs, spawnParticle);

        SetStageData(1);
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
            int level = GameManager.Instance.GetCurrentStageLevel();
            GameManager.Instance.SetCurrentStageLevel(level++);
            Debug.Log("스테이지 클리어");
        }
    }

    public void PlayStageUIAnim()
    {
        stageUIAnim.Play("Effect");
    }

    public void SetWaveCleared()
    {
        ChangeState(new WaveEndState(this));
    }

    public void SetWaveTextValue()
    {
        waveText.text = $"Wave {currentWaveIndex + 1}";
    }

    public WaveSpawningState GetWaveSpawningState()
    {
        return this.waveSpawningState;
    }

    public void SetWaveSpawningState(WaveSpawningState waveSpawningState)
    {
        this.waveSpawningState = waveSpawningState;
    }

    public void SetStageData(int stageNumber)
    {
        stageData = DataManager.Instance.GetStageData(stageNumber);

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].gameObject.SetActive(false);
        }
        levels[stageNumber-1].gameObject.SetActive(true);
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
