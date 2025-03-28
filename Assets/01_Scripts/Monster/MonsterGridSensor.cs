using System.Collections;
using UnityEngine;

namespace Monster
{
    public class MonsterGridSensor : MonoBehaviour
    {
        private MonsterStateMachine stateMachine;
        private Vector2Int CurrentCell;
        private Vector2Int FrontCell;

        private readonly WaitForSeconds checkDelay = new(0.01f);
        private Coroutine checkRoutine;
        
        public Transform Target { get; private set; } = null; // TODO: Unit 클래스가 생기면 변경
        public bool IsArrived { get; private set; }
        
        // TODO: 만약 타겟이 죽는다면?
        // 다시 프론트 셀을 체크하는 루틴을 돌려야 한다.
        // Target = null;
        // TODO: currentCell에 유닛이 배치된다면?
        
        public void Init(MonsterStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            checkRoutine = StartCoroutine(CheckFrontCell());
        }
        
        private IEnumerator CheckFrontCell()
        {
            var gridManager = Test_GridManager.Instance;
            while (true)
            {
                CurrentCell = gridManager.WorldToGridCell(stateMachine.Tr.position);
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
                if (stateMachine.Tr.position.x - gridManager.GetCellCenter(CurrentCell).x < 0.02f)
                {
                    IsArrived = true;
                    yield break;
                }

                yield return checkDelay;
            }
        }
    }
}
