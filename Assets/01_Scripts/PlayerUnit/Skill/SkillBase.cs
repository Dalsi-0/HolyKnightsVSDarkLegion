using UnityEngine;

public abstract class SkillBase
{
    protected PlayerUnit owner;
    protected float cooldown;
    protected float currentCooldown;
    protected GameObject effectPrefab;
    protected float effectDuration = 2f; // 이펙트 지속 시간 (필요에 따라 상속 클래스에서 변경 가능)

    public SkillBase(PlayerUnit owner, float cooldown, GameObject effectPrefab)
    {
        this.owner = owner;
        this.cooldown = cooldown;
        this.currentCooldown = 0f;
        this.effectPrefab = effectPrefab;
        
        // 스킬 이펙트 프리팹이 있으면 풀 초기화
        if (this.effectPrefab != null)
        {
            ProjectilePoolManager.Instance.CreatePool(this.effectPrefab, 3);
        }
    }

    public virtual bool CanUse()
    {
        return currentCooldown <= 0;
    }

    public virtual void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0)
            currentCooldown -= deltaTime;
    }

    public virtual void Activate()
    {
        if (!CanUse()) return;
        
        owner.CurrentState = UnitState.UsingSkill;
        owner.GetAnimationController().SetSkillAnimation(true);
        currentCooldown = cooldown;
        
        // 상속된 클래스에서 이 메서드를 오버라이드하여 스킬 로직 구현
    }

    public virtual void ExecuteEffect()
    {
        // 스킬 이펙트 실행 로직
        // 상속된 클래스에서 오버라이드
    }

    public virtual void CompleteSkill()
    {
        owner.CurrentState = UnitState.Idle;
        owner.GetAnimationController().SetSkillAnimation(false);
    }

    protected virtual void SpawnEffect()
    {
        if (effectPrefab != null)
        {
            Transform firePoint = owner.transform.Find("FirePoint");
            Vector3 spawnPosition = firePoint != null ? firePoint.position : owner.transform.position;
            
            // 오브젝트 풀에서 이펙트 가져오기
            GameObject effect = ProjectilePoolManager.Instance.GetFromPool(
                effectPrefab, 
                spawnPosition, 
                Quaternion.identity
            );
            
            // 일정 시간 후 풀로 반환
            ProjectilePoolManager.Instance.ReturnToPoolWithDelay(effect, effectDuration);
        }
    }

    protected virtual void SpawnEffectAtPosition(Vector3 position)
    {
        if (effectPrefab != null)
        {
            // 오브젝트 풀에서 이펙트 가져오기
            GameObject effect = ProjectilePoolManager.Instance.GetFromPool(
                effectPrefab, 
                position, 
                Quaternion.identity
            );
            
            // 일정 시간 후 풀로 반환
            ProjectilePoolManager.Instance.ReturnToPoolWithDelay(effect, effectDuration);
        }
    }
}