using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
    [SerializeField] private float manaRecoveryAmount = 10f;

    private UnitState _currentState = UnitState.Idle;
    private float _currentHP;
    private Transform _currentTarget;

    // 컴포넌트들
    private UnitAnimationController _animationController;
    private UnitAttackController _attackController;
    private UnitSkillController _skillController;
    private UnitStateController _stateController;

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
        _skillController.Initialize(this, unitData, skillEffectPrefab, manaRecoveryAmount, skillCooldown);
        _stateController.Initialize(this, _attackController, _skillController, canUseBasicAttack, canUseSkill);
    }

    private void ValidateReferences()
    {
        if (unitData == null)
            Debug.LogError("UnitData가 할당되지 않았습니다!");
        if (projectilePrefab == null)
            Debug.LogError("ProjectilePrefab이 할당되지 않았습니다!");
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
        // 추가 죽음 처리 로직
        Destroy(gameObject, 1f); // 애니메이션 재생 시간 고려
    }

    // 애니메이션 이벤트 수신 메서드 (브릿지 역할)
    public void OnAttackAnimationEventReceived()
    {
        _attackController.FireProjectile();
    }

    public void OnSkillAnimationEvent()
    {
        _skillController.ExecuteSkillEffect();
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
}