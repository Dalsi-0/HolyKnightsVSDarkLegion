using Monster;
using UnityEngine;

namespace Monster
{
    public class MonsterAttackState : MonsterBaseState
    {
        // Start is called before the first frame update
        public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly string attackAnimName = "Attack";
        
        public override void Enter()
        {
            stateMachine.MoveSpeedModifier = 0f;
            StartAnimation(hashAttack);
        }

        public override void Update()
        {
            base.Update();
            if (GetNormalizedTime(attackAnimName) > 0.9f)
            {
                stateMachine.ChangeState(stateMachine.idleState);
            }
        }

        public override void Exit()
        {
            StopAnimation(hashAttack);
        }
    }
}