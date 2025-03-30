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

        private IAttackRangeCalc attackRangeCalc;
        public Transform[] Targets { get; private set; } = null; // TODO: Unit 클래스가 생기면 변경
        public bool IsAttackable { get; private set; }
        public bool IsArrived { get; private set; }
        
        // TODO: 만약 타겟이 죽는다면?
        // 다시 프론트 셀을 체크하는 루틴을 돌려야 한다.
        // Target = null;
        // TODO: currentCell에 유닛이 배치된다면?
        
        public void Init(MonsterStateMachine monsterStateMachine)
        {
            stateMachine = monsterStateMachine;
            InitAttackInfo(stateMachine.MonsterData);
            checkRoutine = StartCoroutine(CheckFrontCell());
        }
        
        private void InitAttackInfo(MonsterSO monsterData)
        {
            Targets = new Transform[monsterData.MonsterAtkRange];
            attackRangeCalc = monsterData.MonsterAttackRangeType switch
            {
                ATTACK_RANGE_TYPE.SINGLE     => new SingleAttack(),
                ATTACK_RANGE_TYPE.VERTICAL   => new VerticalAttack(),
                ATTACK_RANGE_TYPE.HORIZONTAL => new HorizontalAttack(),
                _                            => attackRangeCalc
            };
        }
        
        private IEnumerator CheckFrontCell()
        {
            var gridManager = Test_GridManager.Instance;
            while (true)
            {
                Debug.Log(UnitManager.Instance.GetGridIndex(transform.position));
                CurrentCell = gridManager.WorldToGridCell(stateMachine.Tr.position);
                FrontCell = CurrentCell + Vector2Int.left;
                
                var target = gridManager.IsUnitAt(FrontCell);
                if (target)
                {
                    GetTargets();
                    IsAttackable = true;
                    checkRoutine = StartCoroutine(CheckArrived());
                    yield break;
                }
                
                yield return checkDelay;
            }
        }

        private void GetTargets()
        {
            var cells = attackRangeCalc.GetTargetCells(CurrentCell);
            for (int i = 0; i < cells.Length; i++)
            {
                Targets[i] = Test_GridManager.Instance.IsUnitAt(cells[i]);
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
