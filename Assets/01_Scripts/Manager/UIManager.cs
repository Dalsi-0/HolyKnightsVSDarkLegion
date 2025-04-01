using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}