using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStartScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageText;
    public void Start()
    {
        SoundManager.Instance.SetBgm(0);
    }

    /// <summary>
    /// 시작 씬 스테이지 텍스트 변경 메소드
    /// </summary>
    public void StageUpdate()
    {
        stageText.text = $"(STAGE : {GameManager.Instance.GetCurrentStageLevel()})"; // 스테이지 값 {StageData} 기입 필요.
    }

    
}
