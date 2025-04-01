using UnityEngine;

namespace Monsters
{
    // 최초 1회 특수공격을 하는 웨어울프용 공격 상태
    public class WereWolfAttackState : MonsterAttackState
    {
        public WereWolfAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly int hashSpecialAttack = Animator.StringToHash("SpecialAttack");
        private const string normalAttackName = "Attack";
        private const string specialAttackName = "SpecialAttack";

        private int hashAnim;
        private string currentAttackName;

        public override void Enter()
        {
            var wolfFSM = (WereWolfStateMachine)stateMachine;
            stateMachine.SetSpeedModifier(0f);

            hashAnim = hashAttack;
            currentAttackName = normalAttackName;
            if (wolfFSM.IsFirstAttack)
            {
                hashAnim = hashSpecialAttack;
                currentAttackName = specialAttackName;
            }

            StartAnimation(hashAnim);
        }

        public override void Update()
        {
            // 애니메이션이 끝나면 Idle 상태로 전환
            if (GetNormalizedTime(currentAttackName) > 0.95f)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }

        public override void Exit()
        {
            StopAnimation(hashAnim);
        }
    }
}