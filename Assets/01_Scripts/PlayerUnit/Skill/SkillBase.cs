using UnityEngine;

public abstract class SkillBase
{
    protected PlayerUnit owner;
    protected float cooldown;
    protected float currentCooldown;
    protected GameObject effectPrefab;

    public SkillBase(PlayerUnit owner, float cooldown, GameObject effectPrefab)
    {
        this.owner = owner;
        this.cooldown = cooldown;
        this.currentCooldown = 0f;
        this.effectPrefab = effectPrefab;
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
            GameObject effect = Object.Instantiate(effectPrefab, spawnPosition, Quaternion.identity);
            Object.Destroy(effect, 2f);
        }
    }
}