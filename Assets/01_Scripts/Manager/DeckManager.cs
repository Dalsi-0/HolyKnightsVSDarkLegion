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
    [SerializeField] private Dictionary<string, bool> ownedCards; // 소지 카드
    [SerializeField] private Dictionary<string, bool> deck; // 전체카드
    protected override void Awake()
    {
        base.Awake();
        ownedCards = new Dictionary<string, bool>();
        deck = new Dictionary<string, bool>();
        string filePath = Path.Combine(Application.persistentDataPath, ownedCardName + ".json");
        // 파일 불러오기
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            List<CardCollection> cardList = JsonUtility.FromJson<Wrapper<CardCollection>>(jsonData)?.Items;
            foreach (var card in cardList)
            {
                ownedCards[card.unitID] = card.canUse;
            }
        }
        else
        {
            Debug.Log("파일 없음");
            // 기본 값 설정, 
            ownedCards["PK_001"] = true;
            ownedCards["PK_002"] = true;
            ownedCards["PK_003"] = true;
            // 기본 파일 생성
            string json = "{\"Items\":[{\"unitID\":\"PK_001\",\"canUse\":true},{\"unitID\":\"PK_002\",\"canUse\":true},{\"unitID\":\"PK_003\",\"canUse\":true}]}";
            File.WriteAllText(filePath, json);
        }
        // 덱 정보 설정
        int unitCount = 9;
        for (int i = 1; i < unitCount + 1; i++)
        {
            // 000 자리에 맞춰
            string cardName = "PK_" + i.ToString("D3");
            // 데이터 있으면 불러오기
            if (ownedCards.ContainsKey(cardName) && ownedCards[cardName])
            {
                deck[cardName] = ownedCards[cardName];
            }
            // 없으면 사용 불가
            else
                deck[cardName] = false;
        }
    }
    public void SetUse(string unitName)
    {
        // 사용 가능
        ownedCards[unitName] = true;
    }
    public void AddCard(string unitName, bool active = false)
    {
        // 미사용 상태로 추가
        ownedCards[unitName] = active;
    }

    // 생성자에 데이터 세팅
    public void LoadInfo(UnitCreator unitCreator)
    {
        if (unitCreator != null)
        {
            unitCreator.ClearDeck();
            foreach (var card in ownedCards)
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
    public Dictionary<string, bool> GetOwne()
    {
        return ownedCards;
    }
}
[Serializable]
public class Wrapper<T>
{
    public List<T> Items;
}
// JSON 변환용 클래스
[Serializable]
public class CardCollection
{
    public string unitID;
    public bool canUse;
}
