using UnityEngine;
using System.Collections.Generic;

public class KnockbackSkill : SkillBase
{
    private float knockbackForce;  // 넉백 힘
    private float damageAmount;    // 데미지 양
    private float checkRadius;     // 적 감지 범위
    private LayerMask enemyLayer;  // 적 레이어

    public KnockbackSkill(PlayerUnit owner, float cooldown, GameObject effectPrefab,
                         float knockbackForce, float damageAmount, float checkRadius)
        : base(owner, cooldown, effectPrefab)
    {
        this.knockbackForce = knockbackForce;
        this.damageAmount = damageAmount;
        this.checkRadius = checkRadius;
        this.enemyLayer = LayerMask.GetMask("Default");
    }

    public override bool CanUse()
    {
        // 기본 쿨타임 체크만 수행하고 항상 사용 가능하도록 변경
        return base.CanUse();
    }

    public override void Activate()
    {
        // 적이 없어도 항상 활성화 가능하도록 변경
        base.Activate(); // 기본 Activate 로직 실행 (상태 변경, 애니메이션 시작 등)
    }

    public override void ExecuteEffect()
    {
        // 주변의 모든 적 찾기 (2D 물리 사용)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(owner.transform.position, checkRadius, enemyLayer);

        // 가장 가까운 적 찾기
        Collider2D closestEnemy = FindClosestEnemy(hitEnemies);

        // 가까운 적이 있으면 해당 적에게만 넉백 및 데미지 적용
        if (closestEnemy != null)
        {
            // 적 컴포넌트 가져오기
            GameObject enemy = closestEnemy.gameObject;
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            // 항상 오른쪽으로만 넉백 - 직접 위치 이동 사용
            float moveDistance = knockbackForce * 0.5f; // 이동 거리 조정

            if (enemyRb != null)
            {
                // 물리 영향 초기화
                enemyRb.velocity = Vector2.zero;

                // 직접 위치 변경
                Vector2 newPosition = enemyRb.position + new Vector2(moveDistance, 0f);
                enemyRb.position = newPosition;
            }
            else
            {
                // Rigidbody가 없는 경우도 Transform으로 직접 이동
                enemy.transform.position += new Vector3(moveDistance, 0f, 0f);
            }

            // 데미지 적용
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
            }
        }

        // 이펙트 생성
        SpawnEffect();
    }

    // 가장 가까운 적을 찾는 함수
    private Collider2D FindClosestEnemy(Collider2D[] enemies)
    {
        Collider2D closest = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            // 자기 자신은 제외
            if (enemy.gameObject == owner.gameObject)
                continue;

            float distance = Vector2.Distance(owner.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }
}