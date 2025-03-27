using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class WalkState : MonsterBaseState
    {
        public WalkState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashWalk = Animator.StringToHash("Walk");
        
        public override void Enter()
        {
            stateMachine.MoveSpeedModifier = 1f;
            StartAnimation(hashWalk);
        }

        public override void Update()
        {
            base.Update();
            // 앞을 막고 있는 유닛을 체크 -> 막혀있으면 Idle
        }
        
        public override void Exit()
        {
            StopAnimation(hashWalk);
        }
    }
}