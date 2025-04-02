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
    [SerializeField] private TextMeshProUGUI displayStageWaveText;

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

        SetDisplayStageWaveText();
        int currentStage = GameManager.Instance.GetCurrentStageLevel();
        SetStageData(currentStage);
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
            SetDisplayStageWaveText();
            ChangeState(new WaveSpawningState(this, stageData.Waves[currentWaveIndex]));
            currentWaveIndex++;
        }
        else
        {
            StageEnd(true);
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

    private void SetDisplayStageWaveText()
    {
        int currentStage = GameManager.Instance.GetCurrentStageLevel();
        displayStageWaveText.text = $"[  Stage {currentStage}   |   Wave {currentWaveIndex + 1}  ] ";
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

    public void StageEnd(bool isClear)
    {
        stageUIAnim.Play(isClear ? "Clear" : "Lose");

        if (isClear)
        {
            int currentStage = GameManager.Instance.GetCurrentStageLevel();
            int totalStages = DataManager.Instance.GetStageDictionaryLength();

            GameManager.Instance.SetCurrentStageLevel(currentStage >= totalStages ? 1 : currentStage + 1);
        }

        ChangeState(new WaveResultState(this, isClear));
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
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
