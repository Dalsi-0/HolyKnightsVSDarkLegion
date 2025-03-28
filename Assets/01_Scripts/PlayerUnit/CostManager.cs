using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostManager : MonoBehaviour
{
    [SerializeField] private float maxCost = 100f;  // 최대 신성력
    [SerializeField] private float currentCost = 0f;  // 현재 신성력
    [SerializeField] private float naturalRegenRate = 1f;  // 자연 회복률 (초당)

    [Header("UI 요소")]
    [SerializeField] private Slider costSlider;  // 코스트 슬라이더
    [SerializeField] private TextMeshProUGUI costText;  // 코스트 텍스트

    // 코스트 변화시 이벤트 
    public delegate void CostChangedDelegate(float currentCost, float maxCost);
    public event CostChangedDelegate OnCostChanged;

    // 싱글톤 패턴 (선택사항)
    public static CostManager Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 설정 (선택사항)
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // UI 초기화
        UpdateCostUI();
    }

    private void Update()
    {
        // 자연 회복
        RegenerateCost();
    }

    // 자연 회복 처리
    private void RegenerateCost()
    {
        if (currentCost < maxCost)
        {
            AddCost(naturalRegenRate * Time.deltaTime);
        }
    }

    // 코스트 추가 (회복)
    public void AddCost(float amount)
    {
        currentCost = Mathf.Min(currentCost + amount, maxCost);
        UpdateCostUI();

        // 이벤트 발생
        OnCostChanged?.Invoke(currentCost, maxCost);
    }

    // 코스트 사용
    public bool UseCost(float amount)
    {
        // 코스트가 충분한지 확인
        if (currentCost >= amount)
        {
            currentCost -= amount;
            UpdateCostUI();

            // 이벤트 발생
            OnCostChanged?.Invoke(currentCost, maxCost);

            return true;
        }

        return false;
    }

    // 현재 코스트 확인
    public float GetCurrentCost()
    {
        return currentCost;
    }

    // 코스트 비율 확인 (0.0 ~ 1.0)
    public float GetCostRatio()
    {
        return currentCost / maxCost;
    }

    // UI 업데이트
    private void UpdateCostUI()
    {
        if (costSlider != null)
        {
            costSlider.maxValue = maxCost;
            costSlider.value = currentCost;
        }

        if (costText != null)
        {
            costText.text = $"{Mathf.RoundToInt(currentCost)} / {Mathf.RoundToInt(maxCost)}";
        }
    }
}