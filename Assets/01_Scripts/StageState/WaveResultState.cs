using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveResultState : IWaveState
{
    private StageManager stageManager;
    private float leftTime = 0f;
    private float waitTime = 2f; // 2초 후 키 입력 가능

    public WaveResultState(StageManager stageManager)
    {
        this.stageManager = stageManager;
    }

    public void EnterState()
    {
        Time.timeScale = 0;
        leftTime = 0f; // 타이머 초기화
    }

    public void UpdateState()
    {
        leftTime += Time.deltaTime;

        if (leftTime >= waitTime && Input.anyKeyDown)
        {
            Time.timeScale = 1;
            SceneLoadManager.Instance.NumLoadScene(0);
        }
    }
}
