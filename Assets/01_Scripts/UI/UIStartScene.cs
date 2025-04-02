using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStartScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] Button[] buttons;
    GameObject OptionsBtn;

    public void Start()
    {
        OptionsBtn = SoundManager.Instance.transform.GetChild(0).gameObject;
        SoundManager.Instance.SetBgm(0);

        for(int i=0; i<buttons.Length-1; i++)
        {
            buttons[i].onClick.AddListener(() => SetButton(1, 4));
        }

        buttons[2].onClick.AddListener(() => SetOptionsBtn());
        StageUpdate();
    }

    public void SetButton(int stageIndex, int sfxIndex)
    {
        SoundManager.Instance.SetSfx(sfxIndex);
        SceneLoadManager.Instance.NumLoadScene(stageIndex);
    }

    public void SetOptionsBtn(int sfxIndex)
    {
        SoundManager.Instance.SetSfx(sfxIndex);
        OptionsBtn.SetActive(true);
    }

    /// <summary>
    /// 시작 씬 스테이지 텍스트 변경 메소드
    /// </summary>
    public void StageUpdate()
    {
        stageText.text = $"(STAGE : {GameManager.Instance.GetCurrentStageLevel()})"; // 스테이지 값 {StageData} 기입 필요.
    }

    
}
