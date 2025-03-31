using Monsters;
using UnityEngine;

namespace Monsters
{
    public class WereWolfAttackState : MonsterAttackState
    {
        public WereWolfAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

        private readonly int hashSpecialAttack = Animator.StringToHash("SpecialAttack");
        private readonly int hashAttack = Animator.StringToHash("Attack");

        private const string specialAttackName = "SpecialAttack";
        private string currentAnimName;

        public bool IsFirstAttack { get; private set; }

        public override void Enter()
        {
            speedModifier = 0f;
            stateMachine.OnAttack();

            var hashAnim = hashAttack;
            currentAnimName = attackAnimName;
            if (IsFirstAttack)
            {
                hashAnim = hashSpecialAttack;
                currentAnimName = specialAttackName;
                IsFirstAttack = false;
            }

            StartAnimation(hashAnim);
        }

        public override void Update()
        {
            base.Update();

            // 애니메이션이 끝나면 Idle 상태로 전환
            if (GetNormalizedTime(currentAnimName) > 0.95f)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }

        public override void Exit()
        {
            StopAnimation(hashSpecialAttack);
        }
    }
}