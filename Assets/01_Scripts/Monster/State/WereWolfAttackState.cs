using Monsters;
using UnityEngine;

namespace Monsters
{
    public class WereWolfAttackState : MonsterAttackState
    {
        public WereWolfAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

        private readonly int hashSpecialAttack = Animator.StringToHash("SpecialAttack");
        private readonly int hashAttack = Animator.StringToHash("Attack");

        private const string normalAttackName = "Attack";
        private const string specialAttackName = "SpecialAttack";

        public override void Enter()
        {
            var wolfFSM = (WereWolfStateMachine)stateMachine;
            speedModifier = 0f;

            var hashAnim = hashAttack;
            attackAnimName = normalAttackName;
            if (wolfFSM.IsFirstAttack)
            {
                hashAnim = hashSpecialAttack;
                attackAnimName = specialAttackName;
            }

            StartAnimation(hashAnim);
            stateMachine.OnAttack();
        }

        public override void Exit()
        {
            StopAnimation(hashSpecialAttack);
        }
    }
}