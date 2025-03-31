using System.Collections;
using UnityEngine;

namespace Monsters
{
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

        private void FindTarget(PlayerUnit target = null)
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
                currentCell = unitManager.GetGridIndex(transform.position);
                frontCell = currentCell + Vector2Int.left;
                
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

        // 현재 cell을 기준으로 타겟을 다시 설정
        public void SetTarget()
        {
            currentCell = UnitManager.Instance.GetGridIndex(transform.position);
            frontCell = currentCell + Vector2Int.left;
            GetTargets();
        }
        
        public Vector3 GetFrontCellCenter()
        {
            return UnitManager.Instance.GetPosByGrid(frontCell.x, frontCell.y);
        }
        
        private void StopCoroutine()
        {
            StopAllCoroutines();
        }
    }
}
