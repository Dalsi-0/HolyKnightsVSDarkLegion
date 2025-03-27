using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterWalkState : MonsterBaseState
    {
        public MonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine) { }
        private readonly int hashWalk = Animator.StringToHash("Walk");

        private Vector2Int currentCell;
        private Vector2Int frontCell;
        
        public override void Enter()
        {
            stateMachine.MoveSpeedModifier = 1f;
            StartAnimation(hashWalk);
        }

        public override void Update()
        {
            base.Update();
            
            // 유닛을 체크 -> 존재하면 Attack
            currentCell = stateMachine.gridTest.WorldToGridCell(stateMachine.Tr.position);
            frontCell = currentCell + Vector2Int.left;
            
            if (stateMachine.gridTest.IsUnitAt(frontCell))
            {
                if (stateMachine.Tr.position.x - stateMachine.gridTest.GetCellCenter(currentCell).x < 0.01f)
                    stateMachine.ChangeState(stateMachine.idleState);
            }
        }
        
        public override void Exit()
        {
            StopAnimation(hashWalk);
        }
    }
}