using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 덱 리스트에 들어갈 버튼 UI
public class DeckCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image unitImage;
    [SerializeField] Image shadowImage;
    [SerializeField] TextMeshProUGUI coastText;
    [SerializeField] Button clickButton;
    public UnityAction<string> ClickAction; // 클릭 이벤트, 유닛 unitID 반환
    private UnitSO unitSO;
    public string unitID;

    private RectTransform _rt;
    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        if (clickButton != null)
            clickButton.onClick.AddListener(ActCallback);
    }

    public void Setup(UnitSO unit, bool active, Sprite sprite)
    {
        // 이미지 설정
        if (unitImage != null)
        {
            if (unit.UnitSprite != null)
                unitImage.sprite = unit.UnitSprite;

            else if (sprite != null)
                unitImage.sprite = sprite;
        }
        // 데이터 설정
        if (unit != null)
        {
            unitSO = unit;
            coastText.text = unit.UnitSummonCost.ToString();
            unitID = unit.UnitID;
            clickButton.interactable = active;
            shadowImage.enabled = !active;
        }
    }

    public void SetUsable()
    {
        clickButton.interactable = true;
        shadowImage.enabled = false;
    }
    public void ActCallback()
    {
        ClickAction?.Invoke(unitID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 올라간 경우 중심 위치 전달
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        UIManager.Instance.ShowTooltip(pos, eventData, unitSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideTooltip();
    }
}
