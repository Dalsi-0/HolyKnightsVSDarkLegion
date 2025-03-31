using UnityEngine;

namespace Monsters
{
    public class MonsterIdleState : MonsterBaseState
    {
        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashIdle = Animator.StringToHash("Idle");
        
        public override void Enter()
        {
            speedModifier = 0f;
            StartAnimation(hashIdle);
        }
        
        public override void Update()
        {
            base.Update();
            
            // 타겟이 존재하고 공격 가능하다면 공격 상태로 전환
            if (monster.GridSensor.IsAttackable && stateMachine.CanAttack())
                stateMachine.ChangeState(stateMachine.AttackState);
            
            // 앞을 막고 있는 유닛을 죽였는지 체크 -> 죽였으면 이동
            if (!monster.GridSensor.IsAttackable)
                stateMachine.ChangeState(stateMachine.WalkState);
        }
        
        public override void Exit()
        {
            StopAnimation(hashIdle);
        }
    }
}
