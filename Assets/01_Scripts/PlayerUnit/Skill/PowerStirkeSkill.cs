using UnityEngine;
using System.Collections.Generic;

public class PowerStrikeSkill : SkillBase
{
    private float damageAmount;    // 데미지 양
    private float checkRadius;     // 적 감지 범위
    private LayerMask enemyLayer;  // 적 레이어
    private bool attackInRow = true; // 기본적으로 행 방향으로 공격 (가로)
    private float strikeForce;     // 강한 타격감을 위한 이펙트 강도

    public PowerStrikeSkill(PlayerUnit owner, float cooldown, GameObject effectPrefab,
                         float damageAmount, float checkRadius, float strikeForce = 1.0f,
                         bool attackInRow = true)
        : base(owner, cooldown, effectPrefab)
    {
        this.damageAmount = damageAmount;
        this.checkRadius = checkRadius;
        this.enemyLayer = LayerMask.GetMask("Enemy");
        this.attackInRow = attackInRow;
        this.strikeForce = strikeForce;
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

        // 가까운 적이 있으면 해당 적에게 강력한 타격 효과 및 데미지 적용
        if (closestEnemy != null)
        {
            // 적 컴포넌트 가져오기
            GameObject enemy = closestEnemy.gameObject;

            // 타격 방향 계산 (그리드 방향성 고려)
            Vector2 strikeDirection;
            if (attackInRow)
            {
                // 행 방향(가로) 공격
                strikeDirection = new Vector2(1f, 0f);
            }
            else
            {
                // 열 방향(세로) 공격
                strikeDirection = new Vector2(0f, 1f);
            }

            // 시각적 타격 효과 (약간의 흔들림 애니메이션 등이 추가될 수 있음)
            PlayStrikeAnimation(enemy, strikeDirection);

            // 데미지 적용
            Monsters.Monster monster = enemy.GetComponent<Monsters.Monster>();
            if (monster != null)
            {
                // 강력한 타격감을 위해 추가 데미지 보너스 적용 (선택적)
                float finalDamage = damageAmount * (1.0f + (strikeForce * 0.2f));
                monster.StateMachine.OnHit((int)finalDamage);

                // 타격 사운드 효과 (주석 해제 필요)
                //SoundManager.Instance.SetSfx(4); // 더 강한 타격음으로 변경 (인덱스 조정 필요)
            }
        }

        // 강력한 타격 이펙트 생성
        SpawnEffect();
    }

    // 강력한 타격 애니메이션 효과를 재생하는 함수 
    private void PlayStrikeAnimation(GameObject target, Vector2 direction)
    {
        // 타격시 짧은 시각적 효과 (흔들림, 번쩍임 등)
        // 애니메이션 컨트롤러나 셰이더에 접근하여 타격 효과 재생

        // 예시: 타격 위치에 파티클 효과 생성
        if (effectPrefab != null)
        {
            GameObject strikeEffect = GameObject.Instantiate(
                effectPrefab,
                target.transform.position,
                Quaternion.identity
            );

            // 효과 크기를 타격 강도에 비례하게 조정
            strikeEffect.transform.localScale *= strikeForce;

            // 3초 후 자동 제거
            GameObject.Destroy(strikeEffect, 3.0f);
        }

        // 필요한 경우 카메라 흔들림 효과 추가
        //CameraShake.Instance.ShakeCamera(strikeForce * 0.2f, 0.2f);
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