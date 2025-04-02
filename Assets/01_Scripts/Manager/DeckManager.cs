using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Linq;

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
            // 오류 확인용 변수
            bool hasError = false;
            Debug.Log("파일 불러오기");

            try
            {
                // 저장된 파일 불러오기
                string jsonData = File.ReadAllText(filePath);
                PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(jsonData);

                // 기본 정보 불러오기
                string json = Resources.Load<TextAsset>(defaultSettingPath).text;
                PlayerInfo defalutInfo = JsonUtility.FromJson<PlayerInfo>(json);
                // 유효성 검사
                if (playerInfo == null)
                {
                    //파일만 있고 데이터 없으면 기본값
                    throw new Exception("데이터가 NULL임");
                }
                else
                {
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

                    // 유효값인지 확인
                    foreach (var card in deck.ToArray())
                    {
                        bool isIndeck = false;
                        for (int i = 0; i < defalutList.Count; i++)
                        {
                            // 기본값에 포함되면 유효한 값
                            if (card.Key == defalutList[i].unitID)
                            {
                                isIndeck = true;
                                continue;
                            }
                        }
                        if (!isIndeck)
                        {
                            // 유효하지 않으면 제거
                            deck.Remove(card.Key);
                        }
                    }


                    // 사용 목록 불러오기
                    List<string> loadedHands = playerInfo?.InHandCard;
                    List<string> defaultHands = defalutInfo?.InHandCard;

                    // 비어 있다면 기본값으로
                    if (loadedHands.Count < 1)
                    {
                        loadedHands = defaultHands;
                        hasError = true;
                    }
                    // 4개 초과하면 이후는 삭제
                    else if (loadedHands.Count > 4)
                    {
                        loadedHands.RemoveRange(4, loadedHands.Count - 4);
                        hasError = true;
                    }

                    // 유효한지 여부로 추가
                    foreach (var card in loadedHands.ToArray())
                    {
                        // 가지고 있는 카드면 추가
                        if (deck.ContainsKey(card))
                        {
                            inHandCard.Add(card);
                        }
                        else
                        {
                            loadedHands.Remove(card);
                            hasError = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("기타 오류: " + ex.Message);
                // 파싱 오류 발생시 기본값 적용
                Debug.Log("기본값 적용");
                CreateDefalutFile();
                return;
            }
            // 수정 완료된 정보를 파일로 저장
            if (hasError)
                SaveInfo();
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

    public void AddAllCard()
    {
        // 미소지 카드 전부 획득
        List<string> target = new();
        foreach (var card in deck)
        {
            if (!card.Value)
                target.Add(card.Key);
        }
        AddCard(target, true);
    }
    // 특정 카드 덱에 추가 가능하도록(예: 스테이지 보상)
    public void AddCard(List<string> unitName, bool active = false)
    {
        // 미소지 카드만 분별
        foreach (var cardName in unitName.ToList())
        {
            // 이미 가지고 있는 정보면 제외
            if (!deck.ContainsKey(cardName) && deck[cardName])
            {
                // 올바르지 않은값, 이미 가지고 있으면 제외
                unitName.Remove(cardName);
            }
            else
            {
                // 정보 갱신
                deck[cardName] = active;
                // 사용 가능하도록 UI 업데이트
                if(DeckEditor != null)
                    DeckEditor.Reflash(cardName);
            }
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

    // 최근에 사용한 카드 저장
    public void SetInHand(List<string> newValue)
    {
        inHandCard = newValue;
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
