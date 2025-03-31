using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 덱을 편집하는 UI 오브젝트
/// </summary>
public class DeckEditor : MonoBehaviour
{
    [SerializeField] private GameObject deckCardPrefab; // 덱 카드 프리팹
    [SerializeField] private RectTransform deckLayout; // 레이아웃 위치 content
    [SerializeField] private Button startButton; // 완료버튼
    [SerializeField] private UnitCreator unitCreator; // 유닛 생성자

    void Awake()
    {
        startButton.onClick.AddListener(Submit);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 전체 카드 정보 불러오기
        Dictionary<string, bool> allCard = DeckManager.Instance.GetAllCard();
        // 덱 카드 생성
        foreach (var card in allCard)
        {
            var unit = DataManager.Instance.GetUnitData(card.Key);
            var deckcard = Instantiate(deckCardPrefab, deckLayout).GetComponent<DeckCard>();
            deckcard.Setup(unit, card.Value);
            deckcard.ClickAction += AddCard;
        }

        // 소지 카드 생성
        if (unitCreator != null)
        {
            List<string> ownedCards = DeckManager.Instance.GetInHand();
            for (int i = 0; i < ownedCards.Count; i++)
            {
                unitCreator.SpawnUnitCard(ownedCards[i]);
                // 비활성화 상태로 시작
                unitCreator.ChangeMoney(0);
            }
        }
    }
    public void AddCard(string unitID)
    {
        unitCreator.SpawnUnitCard(unitID);
    }
    public void Submit()
    {
        SetActive(false);
        StageManager.Instance.StartNextWave();
    }
    public void SetActive(bool active)
    {
         // 편집 모드 설정/해제
        unitCreator.SetEdit(active);
        gameObject.SetActive(active);
    }
}
