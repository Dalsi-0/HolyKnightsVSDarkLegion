using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int currentStageLevel;


    private void Awake()
    {
        LoadStageLevel();
    }

    private void LoadStageLevel()
    {
        currentStageLevel = PlayerPrefs.GetInt("CurrentStageLevel", 1);
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

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }
}
