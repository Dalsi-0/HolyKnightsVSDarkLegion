using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private GameObject stopButton;
    private List<DeckCard> cards;
    void Awake()
    {
        startButton.onClick.AddListener(Submit);
        startButton.onClick.AddListener(() => SoundManager.Instance.SetBgm(2));
        cards = new();
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
            Sprite sprite = DeckManager.Instance.GetSprite(unit.UnitID);
            deckcard.Setup(unit, card.Value, sprite);
            deckcard.ClickAction += AddCard;
            cards.Add(deckcard);
        }

        // 소지 카드 생성
        if (unitCreator != null)
        {
            List<string> ownedCards = DeckManager.Instance.GetInHand();
            for (int i = 0; i < ownedCards.Count; i++)
            {
                unitCreator.SpawnUnitCard(ownedCards[i]);
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
        StageManager.Instance.ChangeState(new WaveWaitingState(StageManager.Instance));
        DeckManager.Instance.SetInHand(unitCreator.GetInHand());
        DeckManager.Instance.SaveInfo();
        // 사용 가능 갱신
        UnitManager.Instance.ChangeMoney(0);
        // 버튼 활성화
        stopButton.SetActive(true);
        // 사운드 재생
        SoundManager.Instance.SetSfx(4);
        SoundManager.Instance.SetBgm(2);
    }
    public void SetActive(bool active)
    {
        // 편집 모드 설정/해제
        unitCreator.SetEdit(active);
        gameObject.SetActive(active);
    }
    public void Reflash(string unitID)
    {
        foreach (var card in cards)
        {
            if (card.unitID == unitID)
            {
                card.SetUsable();
            }
        }
    }
    public void ReflashAll()
    {
        // 기존 카드 삭제
        foreach (Transform child in deckLayout.transform)
        {
            Destroy(child.gameObject);
        }
        // 전체 카드 정보 불러오기
        Dictionary<string, bool> allCard = DeckManager.Instance.GetAllCard();
        // 덱 카드 생성
        foreach (var card in allCard)
        {
            var unit = DataManager.Instance.GetUnitData(card.Key);
            var deckcard = Instantiate(deckCardPrefab, deckLayout).GetComponent<DeckCard>();
            Sprite sprite = DeckManager.Instance.GetSprite(unit.UnitID);
            deckcard.Setup(unit, card.Value, sprite);
            deckcard.ClickAction += AddCard;
        }
    }
}
