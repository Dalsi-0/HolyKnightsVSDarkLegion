using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

// 애니메이션 이벤트를 받을 컴포넌트
public class AnimationEventReceiver : MonoBehaviour
{
    private PlayerUnit playerUnit;

    private void Start()
    {
        // 부모 오브젝트에서 PlayerUnit 찾기
        playerUnit = GetComponentInParent<PlayerUnit>();
        if (playerUnit == null)
        {
            Debug.LogError("PlayerUnit을 찾을 수 없습니다!");
        }
    }

    // 애니메이션 이벤트에서 호출될 메서드
    public void OnAttackAnimationEvent()
    {
        if (playerUnit != null)
        {
            playerUnit.OnAttackAnimationEventReceived();
        }
    }
}

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;  // 투사체 프리팹
    [SerializeField] private Transform firePoint;  // 발사 위치
    [SerializeField] private float detectionRange = 10f;  // 적 감지 범위
    [SerializeField] private LayerMask enemyLayer;  // 적 레이어
    
    private Animator animator;
    private Vector3 targetPosition; // 목표 위치 저장용
    private Transform currentTarget; // 현재 타겟
    private float attackCooldown = 0f; // 공격 쿨다운
    [SerializeField] private float attackRate = 1f; // 초당 공격 횟수

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
        // 초기 설정 체크
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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
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
                attackCooldown = 1f / attackRate; // 공격 속도에 따른 쿨다운 설정
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
                projectile.Initialize(direction);
            }
        }
    }

    // 기즈모로 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
