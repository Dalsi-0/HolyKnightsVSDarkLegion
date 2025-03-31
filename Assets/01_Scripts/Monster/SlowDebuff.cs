using Monsters;
using UnityEngine;

// 디버프를 적용하기 위한 데이터 인터페이스
public interface DebuffData
{
    DEBUFF_TYPE DebuffType { get; set; }
    Color DebuffColor { get; set; }
    float Duration { get; set; }
    void ApplyDebuff(Monster target);
    void RemoveDebuff(Monster target);
}

// 슬로우 디버프
public class SlowDebuff : DebuffData
{
    public DEBUFF_TYPE DebuffType { get; set; }
    public Color DebuffColor { get; set; }
    public float Duration { get; set; }
    private float slowAmount = 0.5f;

    public SlowDebuff(float duration, Color debuffColor)
    {
        DebuffType = DEBUFF_TYPE.SLOW;
        DebuffColor = debuffColor;
        Duration = duration;
    }

    public void ApplyDebuff(Monster target)
    {
        target.Animator.speed = slowAmount;
        target.StateMachine.SetDebuffSpeedModifier(slowAmount);
        target.StateMachine.SetAttackSpeedModifier(slowAmount);
        target.StateMachine.SetBaseColor(DebuffColor);
    }

    public void RemoveDebuff(Monster target)
    {
        target.Animator.speed = 1f;
        target.StateMachine.SetDebuffSpeedModifier(1f);
        target.StateMachine.SetAttackSpeedModifier(1f);
        target.StateMachine.SetBaseColor(Color.white);
    }
}
