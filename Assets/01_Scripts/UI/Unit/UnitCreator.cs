using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 카드 소환을 관리하는 UI 오브젝트
/// </summary>
public class UnitCreator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; // 카드 프리팹
    [SerializeField] private RectTransform cardLayout; // 카드 부모 레이아웃
    [SerializeField] private TextMeshProUGUI coastText;
    public static int handSize = 4;
    [Header("Deck Info")]
    [SerializeField] private List<string> deckNameList; // 유닛 이름 목록
    public bool loadOnStart; // 시작시 불러올지 여부
    public bool useLocalList; // 인스펙터 창에서 설정한 이름 사용할지 여부
    private Dictionary<string, UnitSO> handList; // 유닛 데이터 목록
    private List<UnitCard> cardList; // 유닛 데이터 목록
    public UnityAction<int> CoastAction; // 코스트 변동시 호출 함수
    private bool isInited;
    public bool onEdited = false; // 수정중인지 여부, 놓을 수 있을지를 판단,
    void Start()
    {
        if (!isInited)
        {
            Init();
        }

        if (loadOnStart)
        {
            // 저장 된 사용가능 카드 불러오기
            if (!useLocalList)
                DeckManager.Instance.LoadInfo(this);

            // 이름으로 카드 생성
            for (int i = 0; i < deckNameList.Count; i++)
            {
                // 이름으로 데이터 불러오기
                SpawnUnitCard(deckNameList[i]);

            }
        }
    }
    private void Init()
    {
        coastText.text = UnitManager.Instance.PlayerMoney.ToString();
        handList = new(handSize);
        cardList = new(handSize);
        isInited = true;
    }

    public void SpawnUnitCard(string unitName)
    {

        // 초기화 검사
        if (!isInited)
        {
            Init();
        }
        // 갯수 검사
        if (handList.Count >= handSize) return;
        // 중복 검사
        if (handList.ContainsKey(unitName)) return;

        // 이름으로 데이터 불러오기
        UnitSO unit = DataManager.Instance.GetUnitData(unitName);
        if (unit.UnitID == null)
            Debug.LogError($"Datamanager에 {unitName} 데이터가 존재하지 않습니다.");
        handList[unit.UnitID] = unit;

        // 카드 생성 및 세팅
        UnitCard card = Instantiate(cardPrefab, cardLayout).GetComponent<UnitCard>();
        Sprite sprite = DeckManager.Instance.GetSprite(unit.UnitID);
        card.SetData(unit, sprite);
        // 함수 등록
        CoastAction += card.UpdateCoast;
        card.OnEndDragActin += DragEndCard;
        cardList.Add(card);
    }
    public void ChangeMoney(int newValue)
    {
        coastText.text = newValue.ToString();
        // 등록 된 이벤트 호출
        CoastAction?.Invoke(newValue);
    }
    public void SetEdit(bool active)
    {
        onEdited = active;
    }

    public void DragEndCard(string unitID, Vector3 pos)
    {

        // 덱 편집 중이라면 해당 카드를 덱에서 빼기
        if (onEdited)
        {
            // UI 삭제, 최소 1개는 보존
            if (cardList.Count > 1)
            {
                handList.Remove(unitID);
                for (int i = 0; i < cardList.Count; i++)
                {
                    if (cardList[i].unitID == unitID)
                    {
                        Destroy(cardList[i].gameObject);
                        cardList.RemoveAt(i);
                    }
                }
            }
            return;
        }

        // 2d여서 z값 보정
        pos.z = 0;

        // 소환 시도
        if (UnitManager.Instance.Spawn(unitID, pos))
        {
            // 소환 성공한 카드의 쿨타임 진행
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i].unitID == unitID)
                {
                    cardList[i].ActiveTimer();
                }
            }
        }
    }

    // 목록 초기화
    public void ClearDeck()
    {
        deckNameList.Clear();
    }

    // 이름 목록에 추가
    public void Add(string cardName)
    {
        deckNameList ??= new List<string>();
        deckNameList.Add(cardName);
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
