using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

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

    // 델리게이트 정의
    public delegate void PlayerDeathHandler(PlayerDeathData deathData);

    // 정적 이벤트 (어디서든 접근 가능)
    public static event PlayerDeathHandler OnPlayerDeath;

    // 인스턴스 이벤트 (특정 유닛의 죽음만 감지하고 싶을 때 사용)
    public event PlayerDeathHandler OnThisPlayerDeath;

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

        // 사망 데이터 생성
        PlayerDeathData deathData = new PlayerDeathData
        {
            DeathPosition = transform.position,
            DeathRotation = transform.rotation,
            UnitData = unitData,
            TimeOfDeath = Time.time,
            LastTarget = _currentTarget,
            RemainingHP = _currentHP
        };

        // 이벤트 발생 (null 체크)
        OnThisPlayerDeath?.Invoke(deathData);
        OnPlayerDeath?.Invoke(deathData);

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

    // 기즈모로 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        if (unitData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, unitData.UnitAtkRange);
        }
    }

    // 필요한 접근자 메서드들
    public Transform GetCurrentTarget() => _currentTarget;
    public void SetCurrentTarget(Transform target) => _currentTarget = target;
    public UnitAnimationController GetAnimationController() => _animationController;
    public UnitSO GetUnitData() => unitData;
    public float GetCurrentHP() => _currentHP;
}