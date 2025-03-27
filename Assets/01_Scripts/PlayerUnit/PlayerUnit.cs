using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public class PlayerUnit : MonoBehaviour, IDamageable
{
    [SerializeField] private UnitSO unitData;  // 유닛 데이터 참조
    [SerializeField] private GameObject projectilePrefab;  // 투사체 프리팹
    [SerializeField] private Transform firePoint;  // 발사 위치
    [SerializeField] private LayerMask enemyLayer;  // 적 레이어
    
    private Animator animator;
    private Vector3 targetPosition; // 목표 위치 저장용
    private Transform currentTarget; // 현재 타겟
    private float attackCooldown = 0f; // 공격 쿨다운
    private float currentHP; // 현재 체력

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
        // 초기 설정 체크
        if (unitData == null)
            Debug.LogError("UnitData가 할당되지 않았습니다!");
        if (projectilePrefab == null)
            Debug.LogError("ProjectilePrefab이 할당되지 않았습니다!");
        if (animator == null)
            Debug.LogError("Animator를 찾을 수 없습니다!");

        // AnimationEventReceiver 컴포넌트가 없다면 추가
        var animatorObj = animator.gameObject;
        if (animatorObj.GetComponent<AnimationEventReceiver>() == null)
        {
            animatorObj.AddComponent<AnimationEventReceiver>();
        }

        // 초기 체력 설정
        currentHP = unitData.UnitHP;
    }

    private void Update()
    {
        DetectAndAttackEnemy();
    }

    private void DetectAndAttackEnemy()
    {
        // 쿨다운 감소
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // 가장 가까운 적 찾기
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, unitData.UnitAtkRange, enemyLayer);
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        // 적이 감지되면 공격 상태 설정
        if (closestEnemy != null)
        {
            currentTarget = closestEnemy;
            targetPosition = closestEnemy.position;
            
            if (attackCooldown <= 0)
            {
                animator.SetBool("IsAttack", true);
                attackCooldown = unitData.UnitAtkDelay; // 공격 딜레이 설정
            }
        }
        else
        {
            currentTarget = null;
            animator.SetBool("IsAttack", false);
        }
    }

    // 애니메이션 이벤트 수신 메서드
    public void OnAttackAnimationEventReceived()
    {
        if (currentTarget != null) // 타겟이 있을 때만 발사
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab != null && currentTarget != null)
        {
            // 발사 위치 설정
            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

            // 타겟 방향으로 발사 방향 계산
            Vector2 direction = ((Vector2)(currentTarget.position - spawnPosition)).normalized;

            // 투사체 생성 및 초기화
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            if (projectileObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(direction, unitData.UnitAtk); // 유닛 공격력 전달
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 사망 처리 로직
        Destroy(gameObject);
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
}
