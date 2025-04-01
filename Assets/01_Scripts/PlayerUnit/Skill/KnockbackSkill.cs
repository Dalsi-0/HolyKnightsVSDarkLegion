using UnityEngine;
using System.Collections.Generic;

public class KnockbackSkill : SkillBase
{
    private float knockbackForce;  // 넉백 힘
    private float damageAmount;    // 데미지 양
    private float checkRadius;     // 적 감지 범위
    private LayerMask enemyLayer;  // 적 레이어
    private bool attackInRow = true; // 기본적으로 행 방향으로 공격 (가로)

    public KnockbackSkill(PlayerUnit owner, float cooldown, GameObject effectPrefab,
                         float knockbackForce, float damageAmount, float checkRadius,
                         bool attackInRow = true)
        : base(owner, cooldown, effectPrefab)
    {
        this.knockbackForce = knockbackForce;
        this.damageAmount = damageAmount;
        this.checkRadius = checkRadius;
        this.enemyLayer = LayerMask.GetMask("Enemy");
        this.attackInRow = attackInRow;
    }

    public override bool CanUse()
    {
        // 기본 쿨타임 체크만 수행
        return base.CanUse();
    }

    public override void Activate()
    {
        // 항상 활성화 가능
        base.Activate();
    }

    public override void ExecuteEffect()
    {
        // 그리드 기반 방향성 고려하여 적 탐지
        Collider2D closestEnemy = FindDirectionalEnemy();

        // 가까운 적이 있으면 해당 적에게만 넉백 및 데미지 적용
        if (closestEnemy != null)
        {
            // 적 컴포넌트 가져오기
            GameObject enemy = closestEnemy.gameObject;
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

            // 이동 방향 계산 (그리드 방향성 고려)
            Vector2 moveDirection;
            if (attackInRow)
            {
                // 행 방향(가로) 공격일 경우 오른쪽으로 넉백
                moveDirection = new Vector2(knockbackForce * 0.5f, 0f);
            }
            else
            {
                // 열 방향(세로) 공격일 경우 위쪽으로 넉백
                moveDirection = new Vector2(0f, knockbackForce * 0.5f);
            }

            // 넉백 적용
            if (enemyRb != null)
            {
                // 물리 영향 초기화 후 직접 위치 변경
                enemyRb.velocity = Vector2.zero;

                // isTrigger가 true인 경우에도 작동하도록 직접 위치 변경
                enemyRb.position = enemyRb.position + moveDirection;
            }
            else
            {
                // Rigidbody가 없는 경우도 Transform으로 직접 이동
                enemy.transform.position += new Vector3(moveDirection.x, moveDirection.y, 0f);
            }

            // 데미지 적용
            Monsters.Monster monster = enemy.GetComponent<Monsters.Monster>();
            if (monster != null)
            {
                monster.StateMachine.OnHit((int)damageAmount);
                //SoundManager.Instance.SetSfx(2);
            }
        }

        // 이펙트 생성
        SpawnEffect();
    }

    // 그리드 기반 방향성을 고려하여 적을 찾는 함수
    private Collider2D FindDirectionalEnemy()
    {
        // 현재 유닛의 그리드 위치 가져오기
        Vector2Int currentGrid = UnitManager.Instance.GetGridIndex(owner.transform.position);

        // 공격 범위를 그리드 거리로 해석
        int gridRange = Mathf.RoundToInt(checkRadius);

        // 넓은 원으로 적들을 검색 (최적화를 위해)
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            owner.transform.position,
            gridRange * Mathf.Max(UnitManager.Instance.stepSize.x, UnitManager.Instance.stepSize.y) * 2,
            enemyLayer
        );

        // 같은 행/열에 있는 가장 가까운 적 찾기
        float closestDistance = float.MaxValue;
        Collider2D closestEnemy = null;

        foreach (var enemy in enemies)
        {
            // 자기 자신은 제외
            if (enemy.gameObject == owner.gameObject)
                continue;

            // 적의 그리드 위치 계산
            Vector2Int enemyGrid = UnitManager.Instance.GetGridIndex(enemy.transform.position);
            bool isInSameLine = false;
            bool isInFrontDirection = false;

            if (attackInRow)
            {
                // 같은 행(y값이 같음)에 있고 공격 범위 내인지 확인
                isInSameLine = (enemyGrid.y == currentGrid.y) &&
                              (Mathf.Abs(enemyGrid.x - currentGrid.x) <= gridRange);

                // 오른쪽 방향에 있는지 확인 (왼쪽/뒤쪽은 공격하지 않음)
                isInFrontDirection = enemyGrid.x > currentGrid.x;
            }
            else
            {
                // 같은 열(x값이 같음)에 있고 공격 범위 내인지 확인  
                isInSameLine = (enemyGrid.x == currentGrid.x) &&
                              (Mathf.Abs(enemyGrid.y - currentGrid.y) <= gridRange);

                // 위쪽 방향에 있는지 확인 (아래쪽/뒤쪽은 공격하지 않음)
                isInFrontDirection = enemyGrid.y > currentGrid.y;
            }

            // 직선 범위 내에 있고, 앞쪽 방향에 있으며, 더 가까운 적인 경우 업데이트
            if (isInSameLine && isInFrontDirection)
            {
                // 실제 거리 계산 (우선순위 결정용)
                float distance = Vector2.Distance(owner.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }
}