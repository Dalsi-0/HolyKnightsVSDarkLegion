using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 덱 리스트에 들어갈 버튼 UI
public class DeckCard : MonoBehaviour
{
    [SerializeField] Image uniyImage;
    [SerializeField] Image shadowImage;
    [SerializeField] TextMeshProUGUI coastText;
    [SerializeField] Button clickButton;
    public UnityAction<string> ClickAction; // 클릭 이벤트, 유닛 unitID 반환
    private UnitSO unitSO;
    private string unitID;
    private Button button;
    void Awake()
    {
        if (clickButton != null)
            clickButton.onClick.AddListener(ActCallback);
    }

    public void Setup(UnitSO unit, bool hideShadow)
    {
        // 이미지 설정
        if (uniyImage != null)
        {
            //uniyImage.sprite = 
        }
        // 데이터 설정
        if (unit != null)
        {
            coastText.text = unit.UnitSummonCost.ToString();
            unitID = unit.UnitID;
            shadowImage.enabled = !hideShadow;
        }
    }

    public void ActCallback()
    {
        ClickAction?.Invoke(unitID);
    }
}
