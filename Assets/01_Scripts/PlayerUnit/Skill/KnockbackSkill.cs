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
        // "Enemy" 레이어를 사용하도록 변경 (레이어가 없다면 Unity에서 생성 필요)
        this.enemyLayer = LayerMask.GetMask("Default");

        // 레이어가 설정되지 않았으면 경고 표시
        if (this.enemyLayer == 0)
        {
            Debug.LogWarning("Enemy 레이어가 설정되지 않았습니다. 프로젝트 설정에서 확인하세요.");
        }
    }

    public override bool CanUse()
    {
        // 기본 쿨타임 체크
        if (!base.CanUse())
            return false;

        // 근접한 적이 있는지 확인
        return IsEnemyInRange();
    }

    private bool IsEnemyInRange()
    {
        // 주변에 적이 있는지 체크 (2D 물리 사용)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(owner.transform.position, checkRadius, enemyLayer);

        // 디버깅을 위한 로그 추가
        if (hitColliders.Length > 0)
        {
            Debug.Log($"감지된 적 수: {hitColliders.Length}");
        }
        else
        {
            Debug.Log($"범위 {checkRadius}m 내에 적이 없습니다. 레이어 마스크: {enemyLayer}");
        }

        // 콜라이더가 0개인 경우 문제 진단
        if (hitColliders.Length == 0)
        {
            // 1. Physics 시스템이 활성화되어 있는지 확인
            Debug.Log("Physics2D 시스템 확인");

            // 2. 더 큰 반경으로 시도
            float largerRadius = checkRadius * 5;
            Collider2D[] withLargerRadius = Physics2D.OverlapCircleAll(owner.transform.position, largerRadius);
            Debug.Log($"더 큰 반경({largerRadius})으로 감지된 콜라이더 수: {withLargerRadius.Length}");

            // 3. 다른 방식으로 시도 (Raycast)
            RaycastHit2D hit = Physics2D.Raycast(owner.transform.position, owner.transform.right, checkRadius);
            if (hit.collider != null)
            {
                Debug.Log($"Raycast로 감지된 오브젝트: {hit.collider.gameObject.name}");
            }
            else
            {
                Debug.Log("Raycast로도 아무것도 감지되지 않음");
            }

            return false;
        }
        return hitColliders.Length > 0;
    }

    public override void Activate()
    {
        // 근접한 적이 없으면 스킬 사용 불가 
        // 주의: 여기 로직이 반대로 되어 있었습니다. 수정했습니다.
        if (!IsEnemyInRange())
        {
            // 플레이어에게 시각적/청각적 피드백 제공 가능
            Debug.Log("적이 근접해 있지 않아 스킬을 사용할 수 없습니다.");
            return;
        }

        base.Activate(); // 기본 Activate 로직 실행 (상태 변경, 애니메이션 시작 등)
    }

    public override void ExecuteEffect()
    {
        Debug.Log("KnockbackSkill.ExecuteEffect 실행");

        // 주변의 모든 적 찾기 (2D 물리 사용)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(owner.transform.position, checkRadius, enemyLayer);
        Debug.Log($"영향을 받는 적: {hitEnemies.Length}개");

        // 각 적에게 넉백 및 데미지 적용
        foreach (var enemyCollider in hitEnemies)
        {
            // 자기 자신은 제외
            if (enemyCollider.gameObject == owner.gameObject)
                continue;

            // 적 컴포넌트 가져오기
            GameObject enemy = enemyCollider.gameObject;
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            // 넉백 방향 계산 (플레이어로부터 적 방향)
            Vector2 knockbackDirection = ((Vector2)enemy.transform.position - (Vector2)owner.transform.position).normalized;

            Debug.Log($"넉백 방향: {knockbackDirection}, 힘: {knockbackForce}");

            // 리지드바디가 있으면 물리적 넉백 적용
            if (enemyRb != null)
            {
                // 넉백 적용 전에 다른 물리 영향 초기화 (선택적)
                enemyRb.velocity = Vector2.zero;

                // 넉백 적용 - 힘 증가
                float actualForce = knockbackForce * 10f; // 힘 증가
                enemyRb.AddForce(knockbackDirection * actualForce, ForceMode2D.Impulse);
                Debug.Log($"{enemy.name}에게 넉백 적용: {knockbackDirection * actualForce}");

                // 대안적인 방법: 직접 속도 설정
                // enemyRb.velocity = knockbackDirection * actualForce;

                // Rigidbody2D 설정 확인
                Debug.Log($"Rigidbody2D 설정: gravityScale={enemyRb.gravityScale}, " +
                         $"constraints={enemyRb.constraints}, " +
                         $"bodyType={enemyRb.bodyType}, " +
                         $"velocity={enemyRb.velocity}");
            }
            else
            {
                Debug.Log($"{enemy.name}에 Rigidbody2D가 없습니다.");

                // 대안: Transform 직접 이동 (Rigidbody2D가 없을 경우)
                enemy.transform.position += (Vector3)(knockbackDirection * (knockbackForce * 0.1f));
            }

            // 데미지 적용
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
                Debug.Log($"{enemy.name}에게 {damageAmount} 데미지 적용");
            }
            else
            {
                Debug.Log($"{enemy.name}에 IDamageable 인터페이스가 구현되어 있지 않습니다.");
            }
        }

        // 이펙트 생성
        SpawnEffect();
        Debug.Log("KnockbackSkill 이펙트 생성 완료");
    }
}