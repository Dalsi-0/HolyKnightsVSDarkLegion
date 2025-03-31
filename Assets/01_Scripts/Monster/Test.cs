using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> list;

    private void Start()
    {
        UnitManager.Instance.ChangeMoney(9999);
        
        for (int i = 0; i < list.Count; i++)
        {
            var index = list[i];
            UnitManager.Instance.Spawn("Soldier", index.x, index.y);
        }
    }
}
