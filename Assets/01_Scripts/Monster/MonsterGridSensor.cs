using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterGridSensor : MonoBehaviour
    {
        public MonsterStateMachine StateMachine;

        [field: SerializeField] public Transform Target { get; private set; } = null; // TODO: Unit 클래스가 생기면 변경
        public Vector2Int CurrentCell { get; private set; }
        public Vector2Int FrontCell { get; private set; }
        public bool IsArrived { get; private set; }
        
        private readonly WaitForSeconds checkDelay = new(0.01f);
        private Coroutine checkRoutine;
        
        // TODO: 만약 타겟이 죽는다면?
        // 다시 프론트 셀을 체크하는 루틴을 돌려야 한다.
        // Target = null;
        // TODO: currentCell에 유닛이 배치된다면?
        public Action OnTargetDead;
        
        public void Start()
        {
            checkRoutine = StartCoroutine(CheckFrontCell());
        }
        
        private IEnumerator CheckFrontCell()
        {
            var gridManager = Test_GridManager.Instance;
            while (true)
            {
                CurrentCell = gridManager.WorldToGridCell(StateMachine.Tr.position);
                FrontCell = CurrentCell + Vector2Int.left;
                
                var target = gridManager.IsUnitAt(FrontCell);
                if (target)
                {
                    Target = target;
                    checkRoutine = StartCoroutine(CheckArrived());
                    yield break;
                }
                
                yield return checkDelay;
            }
        }
        
        private IEnumerator CheckArrived()
        {
            var gridManager = Test_GridManager.Instance;
            while (true)
            {
                if (StateMachine.Tr.position.x - gridManager.GetCellCenter(CurrentCell).x < 0.02f)
                {
                    IsArrived = true;
                    yield break;
                }

                yield return checkDelay;
            }
        }
    }
}
