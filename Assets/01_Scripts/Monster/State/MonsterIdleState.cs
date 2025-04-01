using UnityEngine;

namespace Monsters
{
    public class MonsterIdleState : MonsterBaseState
    {
        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashIdle = Animator.StringToHash("Idle");
        
        public override void Enter()
        {
            stateMachine.SetSpeedModifier(0f);
            StartAnimation(hashIdle);
        }
        
        public override void Update()
        {
            base.Update();
            
            // 타겟이 존재하고 공격 가능하다면 공격 상태로 전환
            if (monster.GridSensor.TriggerUnit && stateMachine.CanAttack())
                stateMachine.ChangeState(stateMachine.AttackState);
            
            // 타겟이 존재하지 않는다면 걷기 상태로 전환
            if (!monster.GridSensor.TriggerUnit)
                stateMachine.ChangeState(stateMachine.WalkState);
        }
        
        public override void Exit()
        {
            StopAnimation(hashIdle);
        }
    }
}
