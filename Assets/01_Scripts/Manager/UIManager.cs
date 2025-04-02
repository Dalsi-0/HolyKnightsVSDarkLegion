using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    private Canvas canvas;
    public Canvas Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }
            return canvas;
        }
    }
    public LoadingThumbnail loadingThumbnail;
    [SerializeField] private GameObject PopupPrefab;
    [SerializeField] private GameObject TooltipPrefab;
    private Tooltip tooltip;
    private RectTransform tooltipTransform;
    public void AddCard(List<string> unitName)
    {
        // 카드 획득 팝업 생성
        if (unitName.Count > 0)
        {
            NewCardPopup popup = Instantiate(PopupPrefab, Canvas.transform).GetComponent<NewCardPopup>();
            for (int i = 0; i < unitName.Count; i++)
            {
                if (popup != null)
                {
                    UnitSO unit = DataManager.Instance.GetUnitData(unitName[i]);
                    Sprite sprite = DeckManager.Instance.GetSprite(unit.UnitID);
                    popup.AddCard(unit, sprite);
                }
            }
        }
    }

    public void ShowTooltip(Vector2 pos, PointerEventData eventData, UnitSO unit)
    {
        // 이미 있으면 활성황
        if (tooltip != null)
            tooltip.gameObject.SetActive(true);

        // 없으면 새로 생성
        if (tooltip == null)
        {
            tooltip = Instantiate(TooltipPrefab, Canvas.transform).GetComponent<Tooltip>();
            tooltipTransform = tooltip.GetComponent<RectTransform>();
        }


        if (tooltip != null)
        {
            // 마우스 위치로 UI 이동
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipTransform.parent as RectTransform,
                pos,
                eventData.pressEventCamera,
                out localPoint
            );
            tooltip.Setup(unit);
            tooltipTransform.anchoredPosition = localPoint;
        }
    }
    public void HideTooltip()
    {
        if (tooltip != null)
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}