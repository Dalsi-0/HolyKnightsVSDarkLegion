using Unity.VisualScripting;
using UnityEngine;

public class UnitSkillController : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private UnitSO _unitData;
    private GameObject _skillEffectPrefab;
    private float _manaRecoveryAmount;
    private float _skillCooldown;
    private float _currentSkillCooldown = 5f;
    private LayerMask _enemyLayer;
    private DivinePowerRecoverySkill _divinePowerRecoverySkill;
    private KnockbackSkill _knockbackSkill;
    private IceBrakeSkill _icebrakeSkill;
    private float _knockbackForce;
    private float _damageAmount;
    private float _checkRadius;

    // 초기화 메서드
    public void Initialize(PlayerUnit playerUnit, UnitSO unitData, GameObject skillEffectPrefab, float manaRecoveryAmount, float skillCooldown,float knockbackForce, float damageAmount, float checkRadius, LayerMask enemyLayer)
    {
        _playerUnit = playerUnit;
        _unitData = unitData;
        _skillEffectPrefab = skillEffectPrefab;
        _manaRecoveryAmount = manaRecoveryAmount;
        _skillCooldown = skillCooldown;
        _knockbackForce = knockbackForce;
        _damageAmount = damageAmount;
        _checkRadius = checkRadius;
        _enemyLayer = enemyLayer;

        // ManaRecoverySkill 인스턴스 생성
        _divinePowerRecoverySkill = new DivinePowerRecoverySkill(_playerUnit, _skillCooldown, _skillEffectPrefab, _manaRecoveryAmount);
        // KnockbackSkill 인스턴스 생성
        _knockbackSkill = new KnockbackSkill(_playerUnit, _skillCooldown, _skillEffectPrefab,
                                            _knockbackForce, _damageAmount, _checkRadius);
        _icebrakeSkill = new IceBrakeSkill(_playerUnit, _skillCooldown, _skillEffectPrefab);
    }

    // UnitStateController에서 Update 메서드에서 호출됨
    public void UpdateSkillCooldown(float deltaTime)
    {
        if (_currentSkillCooldown > 0)
            _currentSkillCooldown -= deltaTime;
    }

    // 스킬 사용 가능 여부를 확인하는 핵심 메서드
    public bool CanUseSkill()
    {
        return _currentSkillCooldown <= 0;
    }

    // 스킬 사용을 시도하는 주요 메서드
    public void TryUseSkill()
    {
        if (!CanUseSkill())
            return;

        // 유닛 상태 변경 및 애니메이션 시작
        _playerUnit.CurrentState = UnitState.UsingSkill;
        _playerUnit.GetAnimationController().SetSkillAnimation(true);

        // 스킬 활성화
        _knockbackSkill.Activate();

        // 쿨다운 초기화
        _currentSkillCooldown = _skillCooldown;
    }


    // 애니메이션 이벤트 핸들러 (PlayerUnit 메서드와 연결됨)
    public void OnSkillStartEvent()
    {
        // 스킬 초기화 로직 (필요시)
    }

    public void OnPriestSkill()
    {
        DivinePowerRecovery();
    }
    public void OnSoldierSkill()
    {
        KnockBackSwing();
    }
    public void OnWizardSkill()
    {
        IceBrake();
    }

    public void OnSkillEndEvent()
    {
        // 대기 상태로 돌아가고 애니메이션 중지
        _playerUnit.CurrentState = UnitState.Idle;
        _playerUnit.GetAnimationController().SetSkillAnimation(false);
    }

    // 실제 스킬 효과를 실행하는 메서드
    private void DivinePowerRecovery()
    {
        if (_playerUnit.CurrentState == UnitState.UsingSkill)
        {
            // ManaRecoverySkill 효과 실행
            if (_divinePowerRecoverySkill != null)
            {
                _divinePowerRecoverySkill.ExecuteEffect();
            }
        }
    }
    private void KnockBackSwing()
    {
        if (_playerUnit.CurrentState == UnitState.UsingSkill)
        {
            // ManaRecoverySkill 효과 실행
            if (_knockbackSkill != null)
            {
                _knockbackSkill.ExecuteEffect();
            }
        }
    }
    private void IceBrake()
    {
        if (_playerUnit.CurrentState == UnitState.UsingSkill)
        {
            // ManaRecoverySkill 효과 실행
            if (_icebrakeSkill != null)
            {
                _icebrakeSkill.ExecuteEffect();
            }
        }
    }
}