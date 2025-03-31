using UnityEngine;

public class WaveWaitingState : IWaveState
{
    private StageManager stageManager;
    private float waitTime = 3f;  // 다음 웨이브까지 대기 시간

    public WaveWaitingState(StageManager stageManager)
    {
        this.stageManager = stageManager;
    }

    public void EnterState()
    {
        Debug.Log("웨이브 대기 상태 " + waitTime + "초 후 웨이브 시작");
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
