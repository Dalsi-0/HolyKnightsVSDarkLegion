using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using TMPro;
public class UnitCard : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Image shadowImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI coastText;

    public UnityAction<string, Vector3> OnEndDragActin; // 드래그 종료시 호출 함수, 스폰
    public string unitID; // 카드 이름
    // 원래 위치
    private Vector3 originPos;
    // 스폰 시간
    private float LastSpawnTime;
    private float coolDown = 3f; // 스폰 딜레이
    private bool canSpawn = true; // 대기시간으로 스폰 가능 여부
    private bool fullCoast = true; // 코스트로 스폰 가능 여부
    private int coast; // 카드의 코스트


    // 컴포넌트
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 시작 위치 저장
        // layout의 자식으로 들어가므로 1 프레임후 위치 저장
        Reposition();
        if (shadowImage != null)
            shadowImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 스폰 가능 여부로 시간 갱신
        if (!canSpawn && fullCoast)
        {
            if (Time.time - LastSpawnTime >= coolDown)
            {
                // 그림자 비활성화
                if (shadowImage != null)
                {
                    shadowImage.enabled = false;
                    shadowImage.fillAmount = 0f;
                }
                // 소환 활성화
                canSpawn = true;
            }
            else
            {
                // 그림자 활성화, 비율로 채우기
                if (shadowImage != null)
                {
                    shadowImage.enabled = true;
                    shadowImage.fillAmount = 1f - (Time.time - LastSpawnTime) / coolDown;
                }
            }
        }
    }
    public void Reposition()
    {
        StartCoroutine(InitializeOriginPosition());
    }
    public void SetData(UnitSO unitSO, Sprite image)
    {
        // 데이터 설정
        coast = (int)unitSO.UnitSummonCost;
        unitID = unitSO.UnitID;
        if (coastText != null)
            coastText.text = coast.ToString();
        if (unitImage != null)
        {
            if (unitSO.UnitSprite != null)
                unitImage.sprite = unitSO.UnitSprite;

            else if (image != null)
                unitImage.sprite = image;
        }
    }

    // 소환 성공 후 그림자 활성화
    public void ActiveTimer()
    {
        // 그림자 이미지 활성화
        if (shadowImage != null)
        {
            shadowImage.enabled = true;
            shadowImage.fillAmount = 1f;
        }
        // 스폰 시간
        LastSpawnTime = Time.time;
        canSpawn = false;
    }
    // 코스트  갱신 이벤트
    public void UpdateCoast(int playerCoast)
    {
        // 플레이어의 현재 코스트를 받아 사용 가능 여부 갱신
        fullCoast = playerCoast >= coast;
        // 그림자 활성화, 비율로 채우기
        if (shadowImage != null)
        {
            if (fullCoast)
            {
                shadowImage.enabled = false;
                shadowImage.fillAmount = 0f;
            }
            else
            {
                StartCoroutine(ShowShadow());
            }
        }
    }
    // 원래 위치 갱신
    public void SetOriginPos()
    {
        originPos = rt.anchoredPosition;
    }



    private IEnumerator InitializeOriginPosition()
    {
        yield return new WaitForEndOfFrame(); // 한 프레임 기다린 후 초기화
        originPos = rt.anchoredPosition;
        //Debug.Log(originPos);
    }
    private IEnumerator ShowShadow()
    {
        yield return new WaitForEndOfFrame(); // 한 프레임 기다린 후 초기화
        shadowImage.enabled = true;
        shadowImage.fillAmount = 1f;
    }


    #region MouseAction
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 시간 검사하고 시간 지나면 드래그 시작
        if (canSpawn && fullCoast)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
    // 드래그 동안 위치 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (canSpawn && fullCoast)
        {
            // 드래그시 위치 이동
            transform.position = eventData.position;
        }
    }

    // 드래그 종료, 이동거리로 스폰 여부 판별
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canSpawn && fullCoast)
        {
            // 특정 거리 이상이면 스폰 시간 갱신
            if (Vector2.Distance(rt.anchoredPosition, originPos) >= 100f)
            {
                // 콜백 실행
                OnEndDragActin?.Invoke(unitID, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            // 원위치로 이동
            rt.anchoredPosition = originPos;
            canvasGroup.blocksRaycasts = true;

        }
    }
    #endregion
}