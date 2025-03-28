using UnityEngine;

public class UnitSkillController : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private UnitSO _unitData;
    private GameObject _skillEffectPrefab;
    private float _manaRecoveryAmount;
    private float _skillCooldown;
    private float _currentSkillCooldown = 0f;

    public void Initialize(PlayerUnit playerUnit, UnitSO unitData, GameObject skillEffectPrefab, float manaRecoveryAmount, float skillCooldown)
    {
        _playerUnit = playerUnit;
        _unitData = unitData;
        _skillEffectPrefab = skillEffectPrefab;
        _manaRecoveryAmount = manaRecoveryAmount;
        _skillCooldown = skillCooldown;
    }

    public void UpdateSkillCooldown(float deltaTime)
    {
        if (_currentSkillCooldown > 0)
            _currentSkillCooldown -= deltaTime;
    }

    public bool CanUseSkill()
    {
        return _currentSkillCooldown <= 0;
    }

    public void TryUseSkill()
    {
        // 여기서 각 유닛 타입에 맞는 스킬 로직 실행
        if (_unitData.UnitAttackType == ATK_TYPE.SPECIAL)
        {
            UseRecoverySkill();
        }
        // 다른 스킬 타입들...
    }

    private void UseRecoverySkill()
    {
        _playerUnit.CurrentState = UnitState.UsingSkill;
        _playerUnit.GetAnimationController().SetSkillAnimation(true);
        _currentSkillCooldown = _skillCooldown;
    }

    public void ExecuteSkillEffect()
    {
        if (_playerUnit.CurrentState == UnitState.UsingSkill)
        {
            if (_unitData.UnitAttackType == ATK_TYPE.SPECIAL)
            {
                RecoverMana();
            }
            // 다른 스킬 타입...

            // 상태 복귀
            _playerUnit.CurrentState = UnitState.Idle;
            _playerUnit.GetAnimationController().SetSkillAnimation(false);
        }
    }

    private void RecoverMana()
    {
        CostManager costManager = CostManager.Instance;
        if (costManager != null)
        {
            costManager.AddCost(_manaRecoveryAmount);

            // 이펙트 생성
            if (_skillEffectPrefab != null)
            {
                Transform firePoint = _playerUnit.transform.Find("FirePoint");
                Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
                GameObject effect = Instantiate(_skillEffectPrefab, spawnPosition, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
    }
}