using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

/// <summary>
/// 플레이어의 덱 데이터를 불러오고 저장하는 매니저
/// </summary>
public class DeckManager : Singleton<DeckManager>
{
    [SerializeField] private List<CardThumbnail> images; // 이미지 목록
    private static string ownedCardName = "PlayerData";
    private static string defaultSettingPath = "DefaultSetting";
    private List<string> inHandCard; // 손에 들고 있는 카드, 최근에 사용한 카드
    private Dictionary<string, bool> deck; // 전체카드, true면 소지
    private DeckEditor deckEditor;
    public DeckEditor DeckEditor
    {
        get
        {
            if (deckEditor == null)
            {
                deckEditor = FindObjectOfType<DeckEditor>();
            }
            return deckEditor;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        inHandCard = new List<string>();
        deck = new Dictionary<string, bool>();
        string filePath = Path.Combine(Application.persistentDataPath, ownedCardName + ".json");
        // 파일 불러오기
        if (File.Exists(filePath))
        {
            Debug.Log("파일 불러오기");
            string jsonData = File.ReadAllText(filePath);

            // 유효성 검사
            PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(jsonData);
            if (playerInfo == null)
            {
                //파일만 있고 데이터 없으면 기본값
                Debug.Log("PlayerInfo 없음");
                CreateDefalutFile();
            }
            else
            {
                // 기본 설정 불러오기
                bool hasError = false;
                string json = Resources.Load<TextAsset>(defaultSettingPath).text;
                PlayerInfo defalutInfo = JsonUtility.FromJson<PlayerInfo>(json);

                // 기본정보 불러오기
                List<CardCollection> defalutList = defalutInfo?.AllCard;

                // 카드 목록 저장
                List<CardCollection> cardList = playerInfo?.AllCard;
                foreach (var card in cardList)
                {
                    deck[card.unitID] = card.canUse;
                }

                // 저장 후 갯수 부족하면 누락 확인
                if (deck.Count < defalutList.Count)
                {
                    // 누락 되면 업데이트
                    foreach (var defaultCard in defalutList)
                    {
                        if (!deck.ContainsKey(defaultCard.unitID))
                        {
                            deck[defaultCard.unitID] = defaultCard.canUse;
                            hasError = true;
                        }
                    }
                }

                // 유효한지 확인
                List<string> errList = new();
                // 유효값인지 확인
                foreach (var card in deck)
                {
                    bool isError = true;
                    for (int i = 0; i < defalutList.Count; i++)
                    {
                        // 기본값에 포함되면 유효한 값
                        if (card.Key == defalutList[i].unitID)
                        {
                            isError = false;
                            continue;
                        }
                    }
                    if (isError)
                    {
                        // 유효한 값이 아니면오류 목룍에 추가
                        // 딕셔너리는 한번에 처리하도록
                        errList.Add(card.Key);
                    }
                }
                // 유효하지 않은 요소 제거
                for (int i = 0; i < errList.Count; i++)
                {
                    Debug.Log("유효하지 않은 요소 : " + errList[i]);
                    deck.Remove(errList[i]);
                    hasError = true;
                }

                // 사용 목록 불러오기
                List<string> hands = playerInfo?.InHandCard;
                for (int i = 0; i < hands.Count; i++)
                {
                    // 가지고 있는 카드면 추가
                    if (deck[hands[i]])
                        inHandCard.Add(hands[i]);
                    else
                        hands.Remove(hands[i]);
                }

                // 수정 완료된 정보를 파일로 저장
                if (hasError)
                    SaveInfo();
            }
        }
        else
        {
            Debug.Log("파일 없음, 기본값 적용");
            CreateDefalutFile();
        }
    }
    private void CreateDefalutFile()
    {
        // 기본 파일 불러오기 생성
        string filePath = Path.Combine(Application.persistentDataPath, ownedCardName + ".json");
        string json = Resources.Load<TextAsset>(defaultSettingPath).text;
        File.WriteAllText(filePath, json);
        ApplyJson(json);
    }
    
    // 특정 카드 덱에 추가 가능하도록(예: 스테이지 보상)
    public void AddCard(string[] unitName, bool active = false)
    {
        for (int i = 0; i < unitName.Length; i++)
        {
            // 미사용 상태로 추가
            deck[unitName[i]] = active;
            // 사용 가능하도록 UI 업데이트
            if(active)
                DeckEditor.Reflash(unitName[i]);
        }
        // 카드 팝업 생성
        if (active)
        {
            UIManager.Instance.AddCard(unitName);
        }
    }

    // 생성자에 데이터 세팅
    public void LoadInfo(UnitCreator unitCreator)
    {
        if (unitCreator != null)
        {
            unitCreator.ClearDeck();
            foreach (var card in deck)
            {
                if (card.Value)
                {
                    unitCreator.Add(card.Key.ToString());
                }
            }
        }
    }

    // 파일로 저장
    public void SaveInfo()
    {
        PlayerInfo newInfo = new PlayerInfo();
        List<CardCollection> cardList = new();
        foreach (var card in deck)
        {
            cardList.Add(new CardCollection(card.Key, card.Value));
        }
        newInfo.AllCard = cardList;
        newInfo.InHandCard = inHandCard;
        string newJson = JsonConvert.SerializeObject(newInfo, Formatting.Indented);
        string filePath = Path.Combine(Application.persistentDataPath, ownedCardName + ".json");
        File.WriteAllText(filePath, newJson);
    }

    public Dictionary<string, bool> GetAllCard()
    {
        return deck;
    }
    public List<string> GetInHand()
    {
        return inHandCard;
    }
    // JSON 파일을 덱에 적용
    public void ApplyJson(string jsonData)
    {
        PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(jsonData);
        // 카드 사용 여부 불러오기
        List<CardCollection> cardList = playerInfo?.AllCard;
        foreach (var card in cardList)
        {
            deck[card.unitID] = card.canUse;
        }
        // 사용 목록 불러어기
        List<string> hands = playerInfo?.InHandCard;
        for (int i = 0; i < hands.Count; i++)
        {
            inHandCard.Add(hands[i]);
        }
    }

    public Sprite GetSprite(string id)
    {
        foreach (var thumbnail in images)
        {
            if (thumbnail.unitID == id)
                return thumbnail.thumbnail;
        }
        return null;
    }

    public void SetActiveEditor(bool active)
    {
        DeckEditor.SetActive(active);
    }
}
// JSON 변환용 클래스
[Serializable]
public class PlayerInfo
{
    public List<CardCollection> AllCard; // 전체 카드 목록
    public List<string> InHandCard; // 전체 카드 목록
}

// JSON 변환용 클래스
[Serializable]
public class CardCollection
{
    public string unitID; // 카드 ID
    public bool canUse; // 카드 사용 여부
    public CardCollection(string id, bool use)
    {
        this.unitID = id;
        this.canUse = use;
    }
}
// 카드 이미지용 클래스
[Serializable]
public class CardThumbnail
{
    public string unitID;
    public Sprite thumbnail;
}
