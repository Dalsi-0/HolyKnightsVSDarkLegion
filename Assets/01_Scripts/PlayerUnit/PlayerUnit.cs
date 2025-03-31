using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum UnitState
{
    Idle,
    Dead,
    Hurt,
    BasicAttack,
    UsingSkill
}

public interface IDamageable
{
    void TakeDamage(float damage);
}

// 사망 데이터를 담을 클래스 정의
[Serializable]
public class PlayerDeathData
{
    public Vector3 DeathPosition;
    public Quaternion DeathRotation;
    public UnitSO UnitData;
    public float TimeOfDeath;
    public Transform LastTarget;
    public float RemainingHP;
}

public class PlayerUnit : MonoBehaviour, IDamageable
{
    [SerializeField] private UnitSO unitData;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject skillEffectPrefab;
    [SerializeField] private LayerMask enemyLayer;

    [Header("유닛 설정")]
    [SerializeField] private bool canUseBasicAttack = true;
    [SerializeField] private bool canUseSkill = true;
    [SerializeField] private float skillCooldown = 5f;
    [Header("사제")]
    [SerializeField] private float manaRecoveryAmount = 10f;
    [Header("넉백 수치")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float damageAmount = 12f;
    [SerializeField] private float checkRadius = 1f;

    private UnitState _currentState = UnitState.Idle;
    private float _currentHP;
    private Transform _currentTarget;

    // 컴포넌트들
    private UnitAnimationController _animationController;
    private UnitAttackController _attackController;
    private UnitSkillController _skillController;
    private UnitStateController _stateController;

    public UnityAction <PlayerUnit>OnPlayerDeadAction;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        ValidateReferences();
        _currentHP = unitData.UnitHP;
    }

    private void Update()
    {
        _stateController.UpdateState();
    }

    private void InitializeComponents()
    {
        // 각 책임별 컴포넌트 초기화
        _animationController = gameObject.AddComponent<UnitAnimationController>();
        _attackController = gameObject.AddComponent<UnitAttackController>();
        _skillController = gameObject.AddComponent<UnitSkillController>();
        _stateController = gameObject.AddComponent<UnitStateController>();

        // 컴포넌트 설정
        _animationController.Initialize(this);
        _attackController.Initialize(this, unitData, projectilePrefab, firePoint, enemyLayer);
        _skillController.Initialize(this, unitData, skillEffectPrefab, manaRecoveryAmount, skillCooldown, knockbackForce, damageAmount, checkRadius, enemyLayer);
        _stateController.Initialize(this, _attackController, _skillController, canUseBasicAttack, canUseSkill);
    }

    private void ValidateReferences()
    {
        if (unitData == null)
            Debug.LogError("UnitData가 할당되지 않았습니다!");
    }

    // PlayerUnit.cs에 추가
    public void SetUnitData(UnitSO data)
    {
        this.unitData = data;
        _currentHP = data.UnitHP;

        // 컴포넌트 재초기화 필요시
        if (_attackController != null)
        {
            _attackController.Initialize(this, unitData, projectilePrefab, firePoint, enemyLayer);
        }
        // 다른 컴포넌트들도 필요에 따라 재초기화
    }

    // 상태 관리
    public UnitState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }

    // IDamageable 인터페이스 구현
    public void TakeDamage(float damage)
    {
        _currentHP -= damage;
        _animationController.SetHitAnimation();
        if (_currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _currentState = UnitState.Dead;
        _animationController.TriggerDeathAnimation();

        OnPlayerDeadAction?.Invoke(this);

        // 추가 죽음 처리 로직
        Destroy(gameObject, 1f); // 애니메이션 재생 시간 고려
    }

    // 애니메이션 이벤트 수신 메서드 (브릿지 역할)
    public void OnAttackAnimationEventReceived()
    {
        _attackController.FireProjectile();
    }

    public void OnSoldierEvent()
    {
        _skillController.OnSoldierSkill();
    }
    public void OnWizardEvent()
    {
        _skillController.OnWizardSkill();
    }

    public void ExecuteSpecificSkill()
    {
        if (_skillController != null)
        {
            _skillController.TryUseSkill();
        }
    }
    public void OnSkillStartEvent()
    {
        if (_skillController != null)
        {
            _skillController.OnSkillStartEvent();
        }
    }

    public void OnPriestEvent()
    {
        if (_skillController != null)
        {
            _skillController.OnPriestSkill();
        }
    }

    public void OnSkillEndEvent()
    {
        if (_skillController != null)
        {
            _skillController.OnSkillEndEvent();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (unitData != null && UnitManager.Instance != null)
        {
            Gizmos.color = Color.red;

            // 현재 유닛의 그리드 위치 가져오기
            Vector2Int currentGrid = UnitManager.Instance.GetGridIndex(transform.position);

            // UnitAtkRange를 그리드 거리로 재해석 (정수로 반올림)
            int gridRange = Mathf.RoundToInt(unitData.UnitAtkRange);

            // 공격 범위 그리드 순회하여 시각화
            for (int x = -gridRange; x <= gridRange; x++)
            {
                for (int y = -gridRange; y <= gridRange; y++)
                {
                    // 맨해튼 거리(Manhattan distance)로 계산하면 다이아몬드 모양 범위가 됨
                    // 체비세프 거리(Chebyshev distance)로 계산하면 정사각형 모양 범위가 됨
                    int distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)); // 체비세프 거리

                    if (distance <= gridRange)
                    {
                        // 범위 내 그리드 좌표 계산
                        int targetX = currentGrid.x + x;
                        int targetY = currentGrid.y + y;

                        // 유효한 그리드 위치인지 확인
                        if (targetX >= 0 && targetX < UnitManager.Instance.tileSize.x &&
                            targetY >= 0 && targetY < UnitManager.Instance.tileSize.y)
                        {
                            // 그리드 중심 위치 가져오기
                            Vector3 targetPos = UnitManager.Instance.GetPosByGrid(targetX, targetY);

                            // 그리드 크기에 맞게 사각형 그리기
                            Gizmos.DrawWireCube(targetPos, new Vector3(
                                UnitManager.Instance.stepSize.x * 0.9f,
                                UnitManager.Instance.stepSize.y * 0.9f,
                                0.1f));
                        }
                    }
                }
            }
        }
    }

    // 필요한 접근자 메서드들
    public Transform GetCurrentTarget() => _currentTarget;
    public void SetCurrentTarget(Transform target) => _currentTarget = target;
    public UnitAnimationController GetAnimationController() => _animationController;
    public UnitSO GetUnitData() => unitData;
    public float GetCurrentHP() => _currentHP;
}