using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveResultState : IWaveState
{
    private StageManager stageManager;
    private bool canInput = false; 

    public WaveResultState(StageManager stageManager)
    {
        this.stageManager = stageManager;
    }

    public void EnterState()
    {
        Time.timeScale = 0;
        stageManager.StartCoroutine(WaitBeforeAllowInput()); // 코루틴 실행
    }

    private IEnumerator WaitBeforeAllowInput()
    {
        yield return new WaitForSecondsRealtime(2f); 
        canInput = true; 
    }

    public void UpdateState()
    {
        if (canInput && Input.anyKeyDown)
        {
            Time.timeScale = 1;
            SceneManager.LoadSceneAsync(0);
        }
    }
}
