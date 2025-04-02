using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveResultState : IWaveState
{
    private StageManager stageManager;
    private bool canInput = false;
    private bool isContinue;

    public WaveResultState(StageManager stageManager, bool isContinue)
    {
        this.stageManager = stageManager;
        this.isContinue = isContinue;
    }

    public void EnterState()
    {
        Time.timeScale = 0;
        stageManager.StartCoroutine(WaitBeforeAllowInput());
    }

    private IEnumerator WaitBeforeAllowInput()
    {
        yield return new WaitForSecondsRealtime(2.2f);
        canInput = true;

        // 승리 했을 때만 팝업 생성
        if (isContinue)
        {
            DeckManager.Instance.AddAllCard();
            DeckManager.Instance.AddHandSize();
            UnitManager.Instance.SetMoney(350);
        }
    }

    public void UpdateState()
    {
        if (!canInput || !Input.anyKeyDown) return;

        Time.timeScale = 1;

        if (isContinue)
        {
            SceneLoadManager.Instance.NumLoadScene(1);
        }
        else
        {
            SceneLoadManager.Instance.LoadMainScene();
        }
        stageManager.DestroyForNextScene();
    }
}
