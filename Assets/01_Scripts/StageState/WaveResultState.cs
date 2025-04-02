using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveResultState : IWaveState
{
    private StageManager stageManager;
    private bool canInput = false;
    private bool isContinue;
    private bool isCardAdded;

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
            isCardAdded = DeckManager.Instance.AddAllCard();
            DeckManager.Instance.AddHandSize();
        }

        if (isContinue && !isCardAdded)
        {
            LoadNextScene();
        }
    }


    public void UpdateState()
    {
        if (!canInput) return;

        if (Input.anyKeyDown)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        stageManager.GetMonsterFactory().ReturnAllMonstersToPool();
        UnitManager.Instance.RemoveAll();
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
