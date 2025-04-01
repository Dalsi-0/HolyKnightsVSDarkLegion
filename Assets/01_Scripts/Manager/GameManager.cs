using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int currentStageLevel;


    private void Start()
    {
        // 저장된 스테이지 레벨 불러오기
        if (PlayerPrefs.HasKey("CurrentStageLevel"))
        {
            currentStageLevel = PlayerPrefs.GetInt("CurrentStageLevel", 1);
        }
    }

    public void GameOver()
    {

    }

    public int GetCurrentStageLevel()
    {
        return currentStageLevel;
    }

    public void SetCurrentStageLevel(int level)
    {
        currentStageLevel = level;
        PlayerPrefs.SetInt("CurrentStageLevel", level); // 저장
        PlayerPrefs.Save();
    }

}
