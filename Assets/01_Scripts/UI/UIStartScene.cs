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
    /// 스테이지 텍스트 메소드
    /// </summary>
    public void StageUpdate()
    {
        stageText.text = $"(STAGE : {1})"; // 스테이지 값 {StageData} 기입 필요.
    }

    
}
