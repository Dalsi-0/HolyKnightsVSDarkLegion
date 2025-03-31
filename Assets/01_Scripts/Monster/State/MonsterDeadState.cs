using Monsters;
using UnityEngine;

public class MonsterDeadState : MonsterBaseState
{
    public MonsterDeadState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    private readonly int hashDead = Animator.StringToHash("Dead");
    private const string deadAnimName = "Dead";
    
    public override void Enter()
    {
        speedModifier = 0f;
        StartAnimation(hashDead);
    }
    
    public override void Update()
    {
        base.Update();
        
        if (GetNormalizedTime(deadAnimName) > 0.95f)
        {
            stateMachine.OnDead();
        }
    }
    
    public override void Exit()
    {
        StopAnimation(hashDead);
    }
}
