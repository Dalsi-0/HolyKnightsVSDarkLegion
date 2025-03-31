using UnityEngine;

namespace Monsters
{
    // AttackRangeType에 따라 타겟의 범위를 계산하는 인터페이스
    public interface IAttackRangeCalc
    {
        Vector2Int[] GetTargetCells(Vector2Int currentCell, int range = 1);
    }

    // 싱글타겟
    public class SingleAttack : IAttackRangeCalc
    {
        public Vector2Int[] GetTargetCells(Vector2Int currentCell, int range = 1)
        {
            return new[] { currentCell + Vector2Int.left };
        }
    }

    // 수직타겟
    public class VerticalAttack : IAttackRangeCalc
    {
        public Vector2Int[] GetTargetCells(Vector2Int currentCell, int range = 1)
        {
            int x = currentCell.x - 1;
            return new[]
            {
                new Vector2Int(x, currentCell.y + 1),
                new Vector2Int(x, currentCell.y),
                new Vector2Int(x, currentCell.y - 1)
            };
        }
    }

    // 수평타겟
    public class HorizontalAttack : IAttackRangeCalc
    {
        public Vector2Int[] GetTargetCells(Vector2Int currentCell, int range = 1)
        {
            var result = new Vector2Int[range];
            for (int i = 0; i < range; i++)
            {
                result[i] = currentCell + Vector2Int.left * (i + 1);
            }

            return result;
        }
    }
}