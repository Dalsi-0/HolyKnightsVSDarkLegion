using UnityEngine;

namespace Monster
{
    public class MonsterWalkState : MonsterBaseState
    {
        public MonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashWalk = Animator.StringToHash("Walk");

        public override void Enter()
        {
            stateMachine.MoveSpeedModifier = 1f;
            StartAnimation(hashWalk);
        }

        public override void Update()
        {
            base.Update();
            
            if (!stateMachine.GridSensor.Target) return; // 타겟이 없으면 리턴
            if (stateMachine.GridSensor.IsArrived)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
        
        public override void Exit()
        {
            StopAnimation(hashWalk);
        }
    }
}