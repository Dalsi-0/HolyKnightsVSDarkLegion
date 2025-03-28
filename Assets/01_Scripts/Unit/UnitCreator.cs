using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 유닛을 소환하는 관리하는 UI 오브젝트, 맵 정보와 UI 연결이 필수
/// </summary>
public class UnitCreator : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab; // 유닛 프리팹
    [SerializeField] private GameObject cardPrefab; // 카드 프리팹
    [SerializeField] private RectTransform cardLayout; // 카드 부모 레이아웃
    [SerializeField] private TextMeshProUGUI coastText;
    public static int deckSize = 4;
    [Header("Deck Info")]
    public bool useLocalList; // 인스펙터 창에서 설정한 이름 사용할지 여부
    [SerializeField] private List<string> deckNameList; // 유닛 이름 목록
    private Dictionary<string, UnitSO> deckList; // 유닛 데이터 목록
    private List<UnitCard> cardList; // 유닛 데이터 목록
    public UnityAction<int> CoastAction; // 코스트 변동시 호출 함수

    [Header("Map Info")]
    public Vector2 startPos; // 좌하단 기준 좌표
    public Vector2Int tileSize; // 타일 총 칸
    public Vector2 stepSize; // 중심점 간 간격
    private bool[,] tileInfo; // 타일위에 소환 가능 여부, false는 이미 유닛 있음
    public int PlayerCoast { get; private set; }

    void Start()
    {
        coastText.text = "0";
        deckList = new(deckSize);
        cardList = new(deckSize);
        tileInfo = new bool[tileSize.x, tileSize.y];
        // 모든 값을 true로 초기화하여 놓을 수 있도록
        for (int i = 0; i < tileInfo.GetLength(0); i++)
        {
            for (int j = 0; j < tileInfo.GetLength(1); j++)
            {
                tileInfo[i, j] = true;
            }
        }

        // 저장 된 사용가능 카드 불러오기
        if (!useLocalList)
            DeckManager.Instance.LoadInfo(this);

        // 이름으로 카드 생성
        for (int i = 0; i < deckNameList.Count; i++)
        {
            // 이름으로 데이터 불러오기
            UnitSO unit = DataManager.Instance.GetUnitData(deckNameList[i]);
            if (unit == null)
                Debug.LogError($"Datamanager에 {deckNameList[i]} 데이터가 존재하지 않습니다.");
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
        // 2d여서 z값 보정
        pos.z = 0;
        // 좌표 범위를 유효한지 검사
        if (pos.x < startPos.x - stepSize.x / 2)
        {
            // x 범위 부족
            return;
        }
        else if (pos.x > startPos.x + stepSize.x * (tileSize.x - 1) + stepSize.x / 2)
        {
            // x 범위 초과
            return;
        }
        else if (pos.y > startPos.y + stepSize.y * (tileSize.y - 1) + stepSize.y / 2)
        {
            // y 범위 초과
            return;
        }
        else if (pos.y < startPos.y - stepSize.y / 2)
        {
            // y 범위 부족
            return;
        }

        // 가장 가까운 중심점 계산

        // 좌표별 차이
        float disX = Mathf.Sqrt((startPos.x - pos.x) * (startPos.x - pos.x));
        float disY = Mathf.Sqrt((startPos.y - pos.y) * (startPos.y - pos.y));

        // 0.5이상은 올리고 미만은 내림, 간격 추가
        int nearX = (int)Mathf.Floor(disX / stepSize.x + 0.5f);
        int nearY = (int)Mathf.Floor(disY / stepSize.y + 0.5f);

        pos.x = startPos.x + stepSize.x * nearX;
        pos.y = startPos.y + stepSize.y * nearY;


        // 소환 가능한 칸인지 판별
        if (!tileInfo[nearX, nearY]) return;

        // 모든 조건을 만족하면 자원 체크
        if (ChangeMoney(-(int)deckList[unitID].UnitSummonCost))
        {
            // 소환 성공한 카드의 쿨타임 진행
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i].unitID == unitID)
                {
                    cardList[i].ActiveTimer();
                }
            }


            // 근처의 중심에 엔티티 소환
            //Debug.Log(deckList[unitID].UnitName + ": " + pos);
            Instantiate(unitPrefab, pos, Quaternion.identity);
            // 배열에 저장
            tileInfo[nearX, nearY] = false;
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
        if (deckNameList == null)
        {
            deckNameList = new List<string>();
        }
        deckNameList.Add(cardName);
    }
}
