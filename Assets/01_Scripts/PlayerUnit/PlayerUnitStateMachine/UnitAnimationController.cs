using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private Animator _animator;

    public void Initialize(PlayerUnit playerUnit)
    {
        _playerUnit = playerUnit;
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator를 찾을 수 없습니다!");
            return;
        }

        // AnimationEventReceiver 컴포넌트가 없다면 추가
        var animatorObj = _animator.gameObject;
        if (animatorObj.GetComponent<AnimationEventReceiver>() == null)
        {
            animatorObj.AddComponent<AnimationEventReceiver>();
        }
    }

    public void SetAttackAnimation(bool isAttacking)
    {
        if (_animator != null)
            _animator.SetBool("IsAttack", isAttacking);
    }

    public void SetSkillAnimation(bool isUsingSkill)
    {
        if (_animator != null)
            _animator.SetBool("IsSkill", isUsingSkill);
    }

    public void SetHitAnimation()
    {
        if (_animator != null)
            _animator.SetTrigger("IsHurt");
    }

    public void TriggerDeathAnimation()
    {
        if (_animator != null)
            _animator.SetBool("IsDead",true);
    }
}