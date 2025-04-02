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


    /// <summary>
    /// 초기 설정
    /// </summary>
    void Start()
    {
        UnitManager.Instance.SetMoney(350);

        stageUIAnim.Play("FadeIn");

        monsterFactory = new MonsterFactory(monsterPrefabs, spawnParticle);

        SetDisplayStageWaveText();
        SetStageData(GameManager.Instance.GetCurrentStageLevel());
    }

    /// <summary>
    /// 현재 상태 업데이트.
    /// </summary>
    void Update()
    {
        currentState?.UpdateState();
    }

    /// <summary>
    /// 현재 웨이브 상태를 변경하고 새 상태를 실행.
    /// </summary>
    public void ChangeState(IWaveState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    /// <summary>
    /// 다음 웨이브를 시작.
    /// </summary>
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

    /// <summary>
    /// 현재 웨이브를 클리어 처리.
    /// </summary>
    public void SetWaveCleared()
    {
        ChangeState(new WaveEndState(this, currentWaveIndex >= stageData.Waves.Count));
    }

    /// <summary>
    /// 현재 웨이브 번호를 UI에 표시.
    /// </summary>
    public void SetWaveTextValue()
    {
        waveText.text = $"Wave {currentWaveIndex + 1}";
    }

    /// <summary>
    /// 스테이지 UI 애니메이션 실행.
    /// </summary>
    public void PlayStageUIAnim()
    {
        stageUIAnim.Play("Effect");
    }

    /// <summary>
    /// 웨이브 스포닝 상태 설정.
    /// </summary>
    public void SetWaveSpawningState(WaveSpawningState waveSpawningState)
    {
        this.waveSpawningState = waveSpawningState;
    }

    /// <summary>
    /// 현재 웨이브 스포닝 상태 반환.
    /// </summary>
    public WaveSpawningState GetWaveSpawningState() => waveSpawningState;

    /// <summary>
    /// 현재 스테이지와 웨이브 정보를 UI에 표시.
    /// </summary>
    private void SetDisplayStageWaveText()
    {
        int currentStage = GameManager.Instance.GetCurrentStageLevel();
        displayStageWaveText.text = $"[  Stage {currentStage}   |   Wave {currentWaveIndex + 1}  ] ";
    }

    /// <summary>
    /// 현재 스테이지 데이터를 설정하고 UI를 업데이트.
    /// </summary>
    public void SetStageData(int stageNumber)
    {
        stageData = DataManager.Instance.GetStageData(stageNumber);

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].gameObject.SetActive(false);
        }
        levels[stageNumber-1].gameObject.SetActive(true);
    }

    /// <summary>
    /// 스테이지 종료 시 처리 (승리 또는 패배).
    /// </summary>
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

    /// <summary>
    /// 씬 변경을 위한 오브젝트 제거.
    /// </summary>
    public void DestroyForNextScene()
    {
        DestroyImmediate(gameObject);
        DestroyImmediate(stageUIAnim.gameObject);
    }

    /// <summary>
    /// 몬스터 팩토리 반환.
    /// </summary>
    public MonsterFactory GetMonsterFactory() => monsterFactory;

    /// <summary>
    /// 몬스터 스폰 위치 배열 반환.
    /// </summary>
    public Transform[] GetSpawnPoints() => spawnPoints;
}
