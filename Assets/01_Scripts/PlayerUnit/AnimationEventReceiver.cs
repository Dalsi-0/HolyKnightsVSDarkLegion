// 애니메이션 이벤트를 받을 컴포넌트
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    private PlayerUnit playerUnit;

    private void Start()
    {
        // 부모 오브젝트에서 PlayerUnit 찾기
        playerUnit = GetComponentInParent<PlayerUnit>();
        if (playerUnit == null)
        {
            Debug.LogError("PlayerUnit을 찾을 수 없습니다!");
        }
    }

    // 애니메이션 이벤트에서 호출될 메서드
    public void OnAttackAnimationEvent()
    {
        if (playerUnit != null)
        {
            playerUnit.OnAttackAnimationEventReceived();
        }
    }
}