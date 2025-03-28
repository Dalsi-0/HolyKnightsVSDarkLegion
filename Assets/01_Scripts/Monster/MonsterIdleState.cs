using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterIdleState : MonsterBaseState
    {
        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashIdle = Animator.StringToHash("Idle");
        
        public override void Enter()
        {
            stateMachine.MoveSpeedModifier = 0f;
            StartAnimation(hashIdle);
        }
        
        public override void Update()
        {
            base.Update();
            
            // 어택 딜레이를 체크 -> 쿨타임이 찼으면 공격
            if (stateMachine.GridSensor.Target && stateMachine.CanAttack())
            {
                stateMachine.ChangeState(stateMachine.attackState);
            }
            
            // 앞을 막고 있는 유닛을 죽였는지 체크 -> 죽였으면 이동
        }
        
        public override void Exit()
        {
            StopAnimation(hashIdle);
        }
    }
}
