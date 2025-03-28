using UnityEngine;

public class UnitStateController : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private UnitAttackController _attackController;
    private UnitSkillController _skillController;
    private bool _canUseBasicAttack;
    private bool _canUseSkill;
    private UnitSO _unitData;

    public void Initialize(PlayerUnit playerUnit, UnitAttackController attackController, UnitSkillController skillController, bool canUseBasicAttack, bool canUseSkill)
    {
        _playerUnit = playerUnit;
        _attackController = attackController;
        _skillController = skillController;
        _canUseBasicAttack = canUseBasicAttack;
        _canUseSkill = canUseSkill;
        _unitData = playerUnit.GetComponent<PlayerUnit>().GetUnitData();
    }

    public void UpdateState()
    {
        
        // 쿨다운 관리
        _attackController.UpdateAttackCooldown(Time.deltaTime);
        _skillController.UpdateSkillCooldown(Time.deltaTime);

        // 공격 가능 상태일 때
        if (_canUseBasicAttack)
        {
            _attackController.DetectAndAttackEnemy();
        }
        // 스킬 사용 가능 상태일 때
        if (_canUseSkill)
        {
            _skillController.TryUseSkill();
        }
        
    }
}