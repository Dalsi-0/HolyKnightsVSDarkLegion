using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStartScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] Button[] Buttons;

    private void Start()
    {
        SoundManager.Instance.SetBgm(0);

        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].onClick.AddListener(() => SetButton(1, 0));
        }

    }
    private void OnDestroy()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].onClick.RemoveAllListeners();
        }
    }

    private void SetButton(int stageIndex, int soundIndex)
    {
        SceneLoadManager.Instance.NumLoadScene(stageIndex);
        SoundManager.Instance.SetSfx(soundIndex);
    }

    /// <summary>
    /// 시작 씬 스테이지 텍스트 변경 메소드
    /// </summary>
    public void StageUpdate()
    {
        stageText.text = $"(STAGE : {GameManager.Instance.GetCurrentStageLevel()})"; // 스테이지 값 {StageData} 기입 필요.
    }


}
