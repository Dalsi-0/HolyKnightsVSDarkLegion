using UnityEngine;

public class DivinePowerRecoverySkill : SkillBase
{
    private float divinePowerRecoveryAmount;

    public DivinePowerRecoverySkill(PlayerUnit owner, float cooldown, GameObject effectPrefab, float manaRecoveryAmount)
        : base(owner, cooldown, effectPrefab)
    {
        this.divinePowerRecoveryAmount = manaRecoveryAmount;
    }

    public override void ExecuteEffect()
    {
        CostManager costManager = CostManager.Instance; // 다른요소로 수정 필요
        if (costManager != null)
        {
            costManager.AddCost(divinePowerRecoveryAmount);
            SpawnEffect();
        }
    }
}