using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UnitCreator : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab; // 유닛 프리팹
    [SerializeField] private GameObject cardPrefab; // 카드 프리팹
    [SerializeField] private RectTransform cardLayout; // 카드 부모 레이아웃
    [SerializeField] private TextMeshProUGUI coastText;
    public int PlayerCoast { get; private set; }

    public static int deckSize = 4;
    public List<string> deckNameList; // 유닛 이름 목록
    private Dictionary<string, UnitSO> deckList; // 유닛 데이터 목록
    private List<UnitCard> cardList; // 유닛 데이터 목록
    public UnityAction<int> CoastAction; // 코스트 변동시 호출 함수



    void Start()
    {
        coastText.text = "0";
        deckList = new(deckSize);
        cardList = new(deckSize);
        for (int i = 0; i < deckNameList.Count; i++)
        {
            // 이름으로 데이터 불러오기
            UnitSO unit = DataManager.Instance.GetUnitData(deckNameList[i]);
            deckList[unit.UnitID] = unit;

            // 카드 생성 및 세팅
            UnitCard card = Instantiate(cardPrefab, cardLayout).GetComponent<UnitCard>();
            card.SetData(unit);
            // 함수 등록
            card.UpdateCoast(PlayerCoast);
            CoastAction += card.UpdateCoast;
            card.OnEndDragActin += TryCreatUnit;
            cardList.Add(card);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeMoney(30);
        }
    }


    // 자원을 추기/소비하는 함수, 소비에 실패하면 falsee 반환
    public bool ChangeMoney(int amount)
    {
        if (amount < 0 && PlayerCoast + amount < 0) return false;
        PlayerCoast += amount;
        coastText.text = PlayerCoast.ToString();
        // 등록 된 이벤트 호출
        CoastAction?.Invoke(PlayerCoast);
        return true;
    }

    public void TryCreatUnit(string unitID, Vector3 pos)
    {
        if (ChangeMoney(-(int)DataManager.Instance.GetUnitData(unitID).UnitSummonCost))
        {
            // 소환 성공한 카드의 쿨타임 진행
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i].unitID == unitID)
                {
                    cardList[i].ActiveTimer();
                }
            }
            // 2d여서 z값 보정
            pos.z = 0;

            // 엔티티 소환
            Debug.Log(deckList[unitID].UnitName + ": " + pos);
            Instantiate(unitPrefab, pos, Quaternion.identity);
        }
        else
        {
            Debug.Log("소환 실패");
        }

    }
}
