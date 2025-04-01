using UnityEngine;

namespace Monsters
{
    public class MonsterWalkState : MonsterBaseState
    {
        public MonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashWalk = Animator.StringToHash("Walk");

        public override void Enter()
        {
            stateMachine.SetSpeedModifier(1f);
            StartAnimation(hashWalk);
        }

        public override void Update()
        {
            base.Update();
            
            // 타겟이 존재하고 목표 지점에 도착했다면 Attack 상태로 전환
            if (!monster.GridSensor.TriggerUnit) return;
            if (monster.GridSensor.IsArrived)
            {
                stateMachine.ChangeState(stateMachine.AttackState);
            }
        }
        
        public override void Exit()
        {
            StopAnimation(hashWalk);
        }
    }
}