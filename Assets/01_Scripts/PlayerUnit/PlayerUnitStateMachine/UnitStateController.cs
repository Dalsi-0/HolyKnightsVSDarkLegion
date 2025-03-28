using UnityEngine;

public class UnitStateController : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private UnitAttackController _attackController;
    private UnitSkillController _skillController;
    private bool _canUseBasicAttack;
    private bool _canUseSkill;

    public void Initialize(PlayerUnit playerUnit, UnitAttackController attackController, UnitSkillController skillController, bool canUseBasicAttack, bool canUseSkill)
    {
        _playerUnit = playerUnit;
        _attackController = attackController;
        _skillController = skillController;
        _canUseBasicAttack = canUseBasicAttack;
        _canUseSkill = canUseSkill;
    }

    public void UpdateState()
    {
        // 쿨다운 관리
        _attackController.UpdateAttackCooldown(Time.deltaTime);
        _skillController.UpdateSkillCooldown(Time.deltaTime);

        // 상태에 따른 행동 결정
        switch (_playerUnit.CurrentState)
        {
            case UnitState.Idle:
                // 공격 가능하면 적 감지 및 공격
                if (_canUseBasicAttack)
                    _attackController.DetectAndAttackEnemy();

                // 스킬 사용 가능하면 스킬 사용 시도
                if (_canUseSkill && _skillController.CanUseSkill())
                    _skillController.TryUseSkill();
                break;

            case UnitState.UsingSkill:
                // 스킬 사용 중일 때의 처리
                break;

            case UnitState.BasicAttack:
                // 기본 공격 중일 때의 처리
                break;

            case UnitState.Hurt:
                // 피해 상태일 때의 처리
                break;

            case UnitState.Dead:
                // 사망 상태일 때의 처리
                break;
        }
    }
}