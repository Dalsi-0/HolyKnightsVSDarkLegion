using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackRangeCalc
{
    Vector2Int[] GetTargetCells(Vector2Int currentCell);
}

public class Single
