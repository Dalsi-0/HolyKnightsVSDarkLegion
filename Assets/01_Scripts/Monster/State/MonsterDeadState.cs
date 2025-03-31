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
            // TODO: 몬스터 제거
            Object.Destroy(stateMachine.gameObject);
        }
    }
    
    public override void Exit()
    {
        StopAnimation(hashDead);
    }
}
