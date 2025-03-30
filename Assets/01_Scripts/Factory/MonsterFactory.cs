using UnityEngine;
using System.Collections.Generic;
using Monster;

public class MonsterFactory
{
    private Dictionary<string, GameObject> monsterPrefabs;

    public MonsterFactory(GameObject[] prefabs)
    {
        // 프리팹을 Dictionary에 등록 (ID를 키로 사용)
        monsterPrefabs = new Dictionary<string, GameObject>();
        foreach (var prefab in prefabs)
        {
            MonsterStateMachine monster = prefab.GetComponent<MonsterStateMachine>();
            if (monster != null && monster.MonsterData != null)
            {
                monsterPrefabs[monster.MonsterData.name] = prefab;
            }
        }
    }

    public MonsterStateMachine SpawnMonster(string monsterID, Vector3 spawnPosition, Transform parent = null)
    {
        if (monsterPrefabs.TryGetValue(monsterID, out GameObject prefab))
        {
            GameObject newMonster = Object.Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
            return newMonster.GetComponent<MonsterStateMachine>();
        }

        Debug.LogError($"{monsterID}이 없습니다");
        return null;
    }
}
