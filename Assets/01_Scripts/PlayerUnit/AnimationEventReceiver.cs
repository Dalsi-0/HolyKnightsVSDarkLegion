using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private SkillBase _currentSkill; // 현재 활성화된 스킬 참조

    private void Start()
    {
        // 상위 계층에서 PlayerUnit 찾기
        _playerUnit = GetComponentInParent<PlayerUnit>();
        if (_playerUnit == null)
            Debug.LogError("AnimationEventReceiver에서 PlayerUnit 참조를 찾을 수 없습니다!");
    }

    // 현재 사용 중인 스킬 설정
    public void SetCurrentSkill(SkillBase skill)
    {
        _currentSkill = skill;
    }

    // 애니메이션에서 호출될 이벤트 메서드들
    public void OnAttackAnimationEvent()
    {
        if (_playerUnit != null)
            _playerUnit.OnAttackAnimationEventReceived();
    }

    // 스킬 단계별 실행을 위한 추가 이벤트들
    public void OnSkillStart()
    {
        if (_playerUnit != null)
            _playerUnit.OnSkillStartEvent();

        // SkillBase를 통한 이벤트 처리 추가
        if (_currentSkill != null)
            _currentSkill.Activate();
    }

    public void OnPriestSkill()
    {
        if (_playerUnit != null)
            _playerUnit.OnPriestEvent();
            SoundManager.Instance.SetSfx(11);
    }

    public void OnAttack()
    {
        if (_playerUnit != null)
            _playerUnit.OnAttackBaseEvent();
            SoundManager.Instance.SetSfx(9);
    }


    public void OnSoldierSkill()
    {
        if (_playerUnit != null)
            _playerUnit.OnSoldierEvent();
            SoundManager.Instance.SetSfx(3);
    }

    public void OnWizardSkill()
    {
        if (_playerUnit != null)
            _playerUnit.OnWizardEvent();
            SoundManager.Instance.SetSfx(11);
    }

    public void OnWizardSound()
    {
        if (_playerUnit != null)
            SoundManager.Instance.SetSfx(10);
    }
    public void OnArrowSound()
    {
        if (_playerUnit != null)
            SoundManager.Instance.SetSfx(8);
    }


    public void OnSkillEnd()
    {
        if (_playerUnit != null)
            _playerUnit.OnSkillEndEvent();

        // SkillBase를 통한 스킬 종료 처리
        if (_currentSkill != null)
        {
            _currentSkill.CompleteSkill();
            _currentSkill = null; // 스킬 사용 완료 후 참조 제거
        }
    }
}