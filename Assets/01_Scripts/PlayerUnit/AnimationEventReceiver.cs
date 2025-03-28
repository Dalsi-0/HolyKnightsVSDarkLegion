using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    private PlayerUnit _playerUnit;

    private void Start()
    {
        // 상위 계층에서 PlayerUnit 찾기
        _playerUnit = GetComponentInParent<PlayerUnit>();

        if (_playerUnit == null)
            Debug.LogError("AnimationEventReceiver에서 PlayerUnit 참조를 찾을 수 없습니다!");
    }

    // 애니메이션에서 호출될 이벤트 메서드들
    public void OnAttackAnimationEvent()
    {
        if (_playerUnit != null)
            _playerUnit.OnAttackAnimationEventReceived();
    }

    public void OnSkillAnimationEvent()
    {
        if (_playerUnit != null)
            _playerUnit.OnSkillAnimationEvent();
    }
}