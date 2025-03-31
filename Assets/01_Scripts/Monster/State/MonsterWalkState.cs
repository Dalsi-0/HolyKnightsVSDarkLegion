using UnityEngine;

namespace Monsters
{
    public class MonsterWalkState : MonsterBaseState
    {
        public MonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashWalk = Animator.StringToHash("Walk");

        public override void Enter()
        {
            speedModifier = 1f;
            StartAnimation(hashWalk);
        }

        public override void Update()
        {
            base.Update();
            
            if (!monster.GridSensor.TriggerUnit) return; // 타겟이 없으면 리턴
            if (monster.GridSensor.IsArrived)
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