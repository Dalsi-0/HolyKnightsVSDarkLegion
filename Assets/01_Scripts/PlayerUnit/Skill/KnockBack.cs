using UnityEngine;
using System.Collections.Generic;

public class Knockback : SkillBase
{
    private float knockbackForce;  // 넉백 힘
    private float damageAmount;    // 데미지 양
    private float checkRadius;     // 적 감지 범위
    private LayerMask enemyLayer;  // 적 레이어

    public Knockback(PlayerUnit owner, float cooldown, GameObject effectPrefab,
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
        // 기본 쿨타임 체크
        if (!base.CanUse())
            return false;

        // 근접한 적이 있는지 확인
        return IsEnemyInRange();
    }

    private bool IsEnemyInRange()
    {
        // 주변에 적이 있는지 체크
        Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, checkRadius, enemyLayer);
        return hitColliders.Length > 0;
    }

    public override void Activate()
    {
        // 근접한 적이 없으면 스킬 사용 불가
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
        // 주변의 모든 적 찾기
        Collider[] hitEnemies = Physics.OverlapSphere(owner.transform.position, checkRadius, enemyLayer);

        // 각 적에게 넉백 및 데미지 적용
        foreach (var enemyCollider in hitEnemies)
        {
            // 적 컴포넌트 가져오기
            GameObject enemy = enemyCollider.gameObject;
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            // 넉백 방향 계산 (플레이어로부터 적 방향)
            Vector3 knockbackDirection = (enemy.transform.position - owner.transform.position).normalized;

            // 리지드바디가 있으면 물리적 넉백 적용
            if (enemyRb != null && !enemyRb.isKinematic)
            {
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
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

    // 추가: EnemyController에 추가해야 하는 메서드 (참고용)
    /*
    public class EnemyController : MonoBehaviour
    {
        // 다른 기존 코드들...
        
        // 넉백을 적용하는 메서드
        public void ApplyKnockback(Vector3 knockbackVector)
        {
            // 현재 이동 중지
            StopMovement();
            
            // 코루틴으로 넉백 모션 실행
            StartCoroutine(KnockbackCoroutine(knockbackVector));
        }
        
        private IEnumerator KnockbackCoroutine(Vector3 knockbackVector)
        {
            float knockbackDuration = 0.2f;
            float elapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + knockbackVector;
            
            // 넉백 상태 설정
            currentState = EnemyState.Stunned;
            
            while (elapsed < knockbackDuration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / knockbackDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // 넉백 후 잠시 스턴 상태 유지
            yield return new WaitForSeconds(0.3f);
            
            // 상태 복구
            currentState = EnemyState.Idle;
        }
    }
    */
}

// UnitSkillController 클래스에 넉백 스킬을 추가하는 예시 코드
/*
// 넉백 스킬 추가
public void AddKnockbackSkill(float knockbackForce, float damageAmount, float checkRadius, float cooldown, GameObject effectPrefab)
{
    KnockbackSkill knockbackSkill = new KnockbackSkill(
        _playerUnit, 
        cooldown, 
        effectPrefab, 
        knockbackForce, 
        damageAmount, 
        checkRadius
    );
    
    // 스킬 타입에 따라 추가 (예: ATK_TYPE.KNOCKBACK이 정의되어 있다고 가정)
    AddSkill(ATK_TYPE.KNOCKBACK, knockbackSkill);
}
*/

// 사용 예시 (스킬 초기화 시)
/*
// UnitSkillController 초기화 메서드 내에서
if (unitData.UnitAttackType == ATK_TYPE.MELEE)
{
    // 넉백 스킬 추가
    float knockbackForce = 10f;   // 넉백 힘
    float damageAmount = 20f;     // 데미지
    float checkRadius = 2.5f;     // 적 감지 범위
    float cooldown = 5f;          // 쿨타임
    
    KnockbackSkill knockbackSkill = new KnockbackSkill(
        playerUnit, 
        cooldown, 
        skillEffectPrefab, 
        knockbackForce, 
        damageAmount, 
        checkRadius
    );
    
    AddSkill(ATK_TYPE.KNOCKBACK, knockbackSkill);
}
*/