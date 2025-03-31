using System;
using System.Collections.Generic;
using Monsters;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> list;
    [SerializeField] private Monster monster;

    private void Start()
    {
        UnitManager.Instance.ChangeMoney(9999);
        
        for (int i = 0; i < list.Count; i++)
        {
            var index = list[i];
            UnitManager.Instance.Spawn("PK_002", index.x, index.y);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var slow = new SlowDebuff(2f, new Color(0.35f, 0.82f, 1f));
            monster.DebuffHandler.ExecuteBuff(slow);
        }
    }
}
