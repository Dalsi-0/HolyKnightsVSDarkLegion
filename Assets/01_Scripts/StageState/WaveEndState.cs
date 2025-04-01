using UnityEngine;

public class WaveEndState : IWaveState
{
    private StageManager stageManager;
    private float endWaitTime = 5f;  // 다음 웨이브 전 대기 시간

    public WaveEndState(StageManager stageManager)
    {
        this.stageManager = stageManager;
    }

    public void EnterState()
    {
        stageManager.PlayStageUIAnim();
        Debug.Log("웨이브 종료 다음 웨이브까지 대기");
    }

    public void UpdateState()
    {
        endWaitTime -= Time.deltaTime;
        if (endWaitTime <= 0)
        {
            stageManager.StartNextWave();
        }
    }
}
