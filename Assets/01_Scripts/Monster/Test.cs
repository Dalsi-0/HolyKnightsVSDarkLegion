using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Vector3 gridOrigin;
    [SerializeField] private int column = 9;
    [SerializeField] private int row = 5;
    private float tileSize = 1.0f;
    private int tilesPerCell = 3;

    private Dictionary<(int,int), bool> grid = new();

    private void Awake()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                grid.Add((x, y), false);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            grid[(7, i)] = true;
        }
    }

    public Vector2Int WorldToGridCell(Vector3 pos)
    {
        float gridSize = tileSize * tilesPerCell;
        int x = Mathf.FloorToInt((pos.x - gridOrigin.x) / gridSize);
        int y = Mathf.FloorToInt((pos.y - gridOrigin.y) / gridSize);
        return new Vector2Int(x, y);
    }
    
    public bool IsUnitAt(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= column || cell.y < 0 || cell.y >= row) return false;
        return grid[(cell.x, cell.y)];
    }
    
    public Vector3 GetCellCenter(Vector2Int cell)
    {
        var gridSize = tileSize * tilesPerCell;
        Vector3 cellOrigin = new Vector3(gridOrigin.x + cell.x * gridSize, gridOrigin.y + cell.y * gridSize, 0);
        return cellOrigin + new Vector3(gridSize / 2, gridSize / 2, 0);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        var gridSize = tileSize * tilesPerCell;

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                // 셀의 좌하단 기준 위치
                Vector3 cellOrigin = new Vector3(
                                                 gridOrigin.x + x * gridSize,
                                                 gridOrigin.y + y * gridSize,
                                                 0);

                // 중심 좌표로 보정
                Vector3 cellCenter = cellOrigin + new Vector3(gridSize / 2f, gridSize / 2f, 0);

                // 중심 기준으로 그리기
                Gizmos.DrawWireCube(cellCenter, new Vector3(gridSize, gridSize, 0));
            }
        }
    }
}