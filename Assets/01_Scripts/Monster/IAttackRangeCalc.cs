using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackRangeCalc
{
    Vector2Int[] GetTargetCells(Vector2Int currentCell, int range = 1);
}

public class SingleAttack : IAttackRangeCalc
{
    public Vector2Int[] GetTargetCells(Vector2Int currentCell, int range = 1)
    {
        return new[] { currentCell + Vector2Int.left };
    }
}

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
