using Monster;
using UnityEngine;

namespace Monster
{
    public class MonsterAttackState : MonsterBaseState
    {
        public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashAttack = Animator.StringToHash("Attack");
        private const string attackAnimName = "Attack";

        public override void Enter()
        {
            stateMachine.MoveSpeedModifier = 0f;
            stateMachine.OnAttack();
            StartAnimation(hashAttack);
        }

        public override void Update()
        {
            base.Update();
            
            // 애니메이션이 끝나면 Idle 상태로 전환
            if (GetNormalizedTime(attackAnimName) > 0.95f)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }

        public override void Exit()
        {
            StopAnimation(hashAttack);
        }
    }
}