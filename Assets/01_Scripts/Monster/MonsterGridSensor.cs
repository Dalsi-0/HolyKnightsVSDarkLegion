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
        public PlayerUnit[] Targets { get; private set; }
        public bool IsAttackable { get; private set; }
        public bool IsArrived { get; private set; }
        
        // TODO: 만약 타겟이 죽는다면?
        // 다시 프론트 셀을 체크하는 루틴을 돌려야 한다.
        // Target = null;
        // TODO: currentCell에 유닛이 배치된다면?
        
        public void Init(MonsterSO monsterData)
        {
            InitAttackInfo(monsterData);
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
                    IsAttackable = true;
                    checkRoutine = StartCoroutine(CheckArrived());
                    yield break;
                }
                
                yield return checkDelay;
            }
        }

        private void GetTargets()
        {
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
            GetTargets();
        }
        
        public Vector3 GetFrontCellCenter()
        {
            return UnitManager.Instance.GetPosByGrid(frontCell.x, frontCell.y);
        }
        
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
