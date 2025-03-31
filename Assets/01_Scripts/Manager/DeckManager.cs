using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 덱 데이터를 불러오고 저장하는 매니저, unitCreator와 연결하여 설정 가능
/// </summary>
public class DeckManager : Singleton<DeckManager>
{
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
            ApplyJson(jsonData);
        }
        else
        {
            Debug.Log("파일 없음, 기본값 적용");
            // 기본 파일 불러오기 생성
            string json = Resources.Load<TextAsset>(defaultSettingPath).text;
            File.WriteAllText(filePath, json);
            ApplyJson(json);
        }
    }
    public void SetUse(string unitName)
    {
        // 사용 가능
        deck[unitName] = true;
    }
    public void AddCard(string unitName, bool active = false)
    {
        // 미사용 상태로 추가
        deck[unitName] = active;
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
}
