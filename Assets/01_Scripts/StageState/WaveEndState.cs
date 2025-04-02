using UnityEngine;

public class WaveEndState : IWaveState
{
    private StageManager stageManager;
    private float endWaitTime = 5f;  // 다음 웨이브 전 대기 시간
    private bool isClear;

    public WaveEndState(StageManager stageManager, bool isClear)
    {
        this.stageManager = stageManager;
        this.isClear = isClear;
    }

    public void EnterState()
    {
        if (!isClear)
        {
            SoundManager.Instance.SetSfx(14);
            stageManager.SetWaveTextValue();
            stageManager.PlayStageUIAnim();
        }
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
