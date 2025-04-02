using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewCardPopup : MonoBehaviour
{
    public GameObject cardPrefab; // 카드 프리팹
    public RectTransform spawnLayout; // 카드 부모
    public Button okButton; // 확인 버튼

    // Start is called before the first frame update
    void Start()
    {
        okButton.onClick.AddListener(OnClikOk);
    }
    public void AddCard(UnitSO unit, Sprite sprite)
    {
        var card = Instantiate(cardPrefab, spawnLayout).GetComponent<DeckCard>();
        card.Setup(unit, true, sprite);
        // 원본을 크게 출력
        card.transform.localScale = new Vector3(4, 4, 1);
    }
    void OnClikOk()
    {
        //Destroy(gameObject);
    }
}
