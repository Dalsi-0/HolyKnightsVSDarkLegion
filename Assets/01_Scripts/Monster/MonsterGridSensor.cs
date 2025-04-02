using System.Collections;
using UnityEngine;

namespace Monsters
{
    // 그리드에 존재하는 유닛을 감지하는 센서
    public class MonsterGridSensor : MonoBehaviour
    {
        private Vector2Int currentCell;
        private Vector2Int frontCell;

        private readonly WaitForSeconds checkDelay = new(0.01f);
        private Coroutine checkRoutine;

        private IAttackRangeCalc attackRangeCalc;
        public PlayerUnit TriggerUnit { get; private set; }
        public PlayerUnit[] Targets { get; private set; }
        public bool IsArrived { get; private set; }
        
        public void Init(MonsterSO monsterData)
        {
            InitAttackInfo(monsterData);
            FindTarget();
        }

        public void FindTarget(PlayerUnit target = null)
        {
            if (TriggerUnit) TriggerUnit.OnPlayerDeadAction -= FindTarget;
                
            TriggerUnit = null;
            checkRoutine = StartCoroutine(CheckFrontCell());
        }
        
        private void InitAttackInfo(MonsterSO monsterData)
        {
            Targets = new PlayerUnit[monsterData.MonsterAtkRange];
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
            var unitManager = UnitManager.Instance;
            while (true)
            {
                // Unit Manager를 통해 자신의 현재 셀을 체크
                currentCell = unitManager.GetGridIndex(transform.position);
                frontCell = currentCell + Vector2Int.left;
                
                // 앞에 유닛이 존재하면 타겟 설정 후 종료
                var target = unitManager.IsOnUnit(frontCell.x, frontCell.y);
                if (target)
                {
                    GetTargets();
                    checkRoutine = StartCoroutine(CheckArrived());
                    yield break;
                }
                
                yield return checkDelay;
            }
        }

        private void GetTargets()
        {
            TriggerUnit = UnitManager.Instance.IsOnUnit(frontCell.x, currentCell.y);
            if (!TriggerUnit) return;
            
            TriggerUnit.OnPlayerDeadAction += FindTarget;
            
            var cells = attackRangeCalc.GetTargetCells(currentCell);
            for (int i = 0; i < cells.Length; i++)
            {
                Targets[i] = UnitManager.Instance.IsOnUnit(cells[i].x, cells[i].y);
            }
        }
        
        private IEnumerator CheckArrived()
        {
            var unitManager = UnitManager.Instance;
            while (true)
            {
                float distance = transform.position.x - unitManager.GetPosByGrid(currentCell.x, currentCell.y).x;
                if (distance < 0.01f)
                {
                    IsArrived = true;
                    yield break;
                }

                yield return checkDelay;
            }
        }
        
        public Vector3 GetFrontCellCenter()
        {
            return UnitManager.Instance.GetPosByGrid(frontCell.x, frontCell.y);
        }

        private void OnDisable()
        {
            if (TriggerUnit) TriggerUnit.OnPlayerDeadAction -= FindTarget;
            StopAllCoroutines();
        }
    }
}
