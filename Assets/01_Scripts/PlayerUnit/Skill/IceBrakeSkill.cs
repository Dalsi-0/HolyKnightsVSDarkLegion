
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceBrakeSkill : SkillBase
{
    private float slowAmount = 0.5f; // 50% 감속
    private float slowDuration = 1.5f; // 감속 지속 시간 (초)
    private float iceDropHeight = 5f; // 얼음이 떨어지는 높이
    private float iceDropSpeed = 10f; // 얼음이 떨어지는 속도

    public IceBrakeSkill(PlayerUnit owner, float cooldown, GameObject effectPrefab)
        : base(owner, cooldown, effectPrefab)
    {
    }

    public override void ExecuteEffect()
    {
        // 기본 이펙트 스폰 (캐스팅 이펙트)
        base.SpawnEffect();

        // 가장 가까운 적 찾기
        Transform targetEnemy = FindClosestEnemy();

        if (targetEnemy != null)
        {
            // 얼음 떨어트리는 효과 시작
            owner.StartCoroutine(DropIceAnimation(targetEnemy));
        }
    }

    private Transform FindClosestEnemy()
    {
        // PlayerUnit의 enemyLayer 사용
        LayerMask enemyLayer = owner.GetEnemyLayer();

        // 범위 내의 모든 적 찾기
        Collider2D[] enemies = Physics2D.OverlapCircleAll(owner.transform.position,
                                                        owner.GetUnitData().UnitAtkRange,
                                                        enemyLayer);

        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(owner.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    private IEnumerator DropIceAnimation(Transform targetEnemy)
    {
        if (targetEnemy == null) yield break;

        // 타겟 위치
        Vector3 targetPosition = targetEnemy.position;

        // 얼음이 떨어지기 시작할 위치 (타겟 위, 높이)
        Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y + iceDropHeight, targetPosition.z);

        // 시각적 효과로 얼음 프리팹 사용 (기본 스킬 이펙트 프리팹 사용)
        GameObject iceObject = null;
        if (effectPrefab != null)
        {
            iceObject = Object.Instantiate(effectPrefab, startPosition, Quaternion.identity);

            // 스케일 조정 (필요한 경우)
            iceObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        // 얼음 떨어지는 애니메이션
        float distanceToTravel = iceDropHeight;
        float travelTime = distanceToTravel / iceDropSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            // 타겟이 없어진 경우
            if (targetEnemy == null) break;

            // 얼음이 없어진 경우
            if (iceObject == null) break;

            // 시간에 따라 얼음 위치 업데이트 (타겟이 움직이더라도 타겟을 따라감)
            float t = elapsedTime / travelTime;
            targetPosition = targetEnemy.position; // 타겟 위치 업데이트
            Vector3 currentDropPosition = new Vector3(
                targetPosition.x,
                Mathf.Lerp(startPosition.y, targetPosition.y, t),
                targetPosition.z
            );

            iceObject.transform.position = currentDropPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 얼음이 지면에 닿음 - 얼음 파괴
        if (iceObject != null)
        {
            Object.Destroy(iceObject);
        }

        // 타겟이 아직 존재하는지 확인
        if (targetEnemy != null)
        {
            // 충돌 이펙트 생성 (같은 이펙트 프리팹 재사용)
            GameObject splashEffect = Object.Instantiate(effectPrefab, targetEnemy.position, Quaternion.identity);
            Object.Destroy(splashEffect, 2f);

            // 충격파 사운드 효과 재생 (필요한 경우)
            AudioSource audioSource = owner.GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }

            // 적에게 감속 효과 적용
            ApplySlowEffect(targetEnemy);

            // 추가: 주변 적에게도 효과 적용 (작은 범위 AoE)
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(targetEnemy.position, 1.5f, owner.GetEnemyLayer());
            foreach (var enemyCollider in nearbyEnemies)
            {
                if (enemyCollider.transform != targetEnemy) // 메인 타겟은 이미 처리했으므로 제외
                {
                    ApplySlowEffect(enemyCollider.transform);
                }
            }
        }
    }

    private void ApplySlowEffect(Transform enemyTransform)
    {
        if (enemyTransform == null) return;

        // 색상 효과 적용 (스프라이트 렌더러가 있는 경우)
        SpriteRenderer renderer = enemyTransform.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.color;
            renderer.color = new Color(0.7f, 0.9f, 1f); // 옅은 푸른색 (얼음 효과)

            // 색상 원상복구를 위한 코루틴
            owner.StartCoroutine(RestoreColor(enemyTransform.gameObject, renderer, originalColor));
        }

        // 적 컴포넌트 가져오기
        MonoBehaviour enemyComponent = enemyTransform.GetComponent<MonoBehaviour>();
        if (enemyComponent != null)
        {
            // 적 이동 속도 감소 메서드 호출 시도
            System.Reflection.MethodInfo slowMethod = enemyComponent.GetType().GetMethod("ApplySlow");
            if (slowMethod != null)
            {
                // ApplySlow 메서드가 있으면 호출
                slowMethod.Invoke(enemyComponent, new object[] { slowAmount, slowDuration });
            }
        }
    }

    private IEnumerator RestoreColor(GameObject enemyObject, SpriteRenderer renderer, Color originalColor)
    {
        if (enemyObject == null || renderer == null) yield break;

        float elapsedTime = 0f;
        Color iceColor = renderer.color;

        while (elapsedTime < slowDuration)
        {
            if (enemyObject == null || renderer == null) yield break;

            float t = elapsedTime / slowDuration;
            renderer.color = Color.Lerp(iceColor, originalColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (renderer != null)
        {
            renderer.color = originalColor;
        }
    }
}