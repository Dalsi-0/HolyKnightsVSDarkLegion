using UnityEngine;

public class WaveWaitingState : IWaveState
{
    private StageManager stageManager;
    private float waitTime = 5f;  // 다음 웨이브까지 대기 시간

    public WaveWaitingState(StageManager stageManager)
    {
        this.stageManager = stageManager;
    }

    public void EnterState()
    {
        SoundManager.Instance.SetSfx(14);
        stageManager.SetWaveTextValue();
        stageManager.PlayStageUIAnim();
    }

    public void UpdateState()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0)
        {
            stageManager.StartNextWave();
        }
    }
}
