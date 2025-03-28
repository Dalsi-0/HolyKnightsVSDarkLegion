using UnityEngine;

public class ManaRecoverySkill : SkillBase
{
    private float manaRecoveryAmount;

    public ManaRecoverySkill(PlayerUnit owner, float cooldown, GameObject effectPrefab, float manaRecoveryAmount)
        : base(owner, cooldown, effectPrefab)
    {
        this.manaRecoveryAmount = manaRecoveryAmount;
    }

    public override void ExecuteEffect()
    {
        CostManager costManager = CostManager.Instance; // 다른요소로 수정 필요
        if (costManager != null)
        {
            costManager.AddCost(manaRecoveryAmount);
            SpawnEffect();
        }
    }
}