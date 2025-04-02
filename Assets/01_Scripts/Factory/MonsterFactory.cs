using System.Collections.Generic;
using UnityEngine;
using Monsters;
using System.Collections;

public class MonsterFactory
{
    private Dictionary<string, MonsterPool> monsterPools;  // 몬스터 ID별 풀을 관리하는 딕셔너리
    private GameObject spawnParticle;

    public MonsterFactory(GameObject[] prefabs, GameObject spawnParticle)
    {
        monsterPools = new Dictionary<string, MonsterPool>();

        // 각 몬스터 프리팹에 대해 풀을 생성
        foreach (var prefab in prefabs)
        {
            Monster monster = prefab.GetComponent<Monster>();
            if (monster != null)
            {
                string monsterID = monster.MonsterId;
                monsterPools[monsterID] = new MonsterPool(prefab, 5);  // 풀의 초기 크기는 5로 설정

            }
        }

        this.spawnParticle = spawnParticle;
    }


    // 몬스터를 소환하는 함수
    public void SpawnMonster(string monsterID, Vector3 spawnPosition, MonoBehaviour helper)
    {
        if (monsterPools.TryGetValue(monsterID, out MonsterPool pool))
        {
            helper.StartCoroutine(SpawnMonsterCoroutine(pool, spawnPosition));
        }
        else
        {
            Debug.LogError("풀에서 찾을 수 없다.");
        }
    }

    private IEnumerator SpawnMonsterCoroutine(MonsterPool pool, Vector3 spawnPosition)
    {
        // 스폰 파티클 생성
        if (spawnParticle != null)
        {
            SoundManager.Instance.SetSfx(13);
            GameObject particle = Object.Instantiate(spawnParticle, spawnPosition, Quaternion.identity);
            Animator particleAnim = particle.GetComponent<Animator>();

            // 애니메이션 길이 계산 후 자동 삭제
            float animTime = particleAnim != null ? particleAnim.GetCurrentAnimatorStateInfo(0).length : 1f;
            Object.Destroy(particle, animTime);
        }

        // 0.5초 대기 후 몬스터 활성화
        yield return new WaitForSeconds(0.5f);

        GameObject monster = pool.GetObject();
        monster.transform.position = spawnPosition;
        monster.SetActive(true);
        monster.GetComponent<Monster>().Init();
    }



    // 풀에 몬스터를 반환하는 함수
    public void ReturnMonsterToPool(GameObject monster)
    {
        monster.SetActive(false);  // 비활성화
        string monsterID = monster.GetComponent<Monster>().MonsterId;

        if (monsterPools.TryGetValue(monsterID, out MonsterPool pool))
        {
            pool.ReturnObject(monster);  // 풀에 반환
        }
        else
        {
            Debug.LogError("이 몬스터의 풀을 찾을 수 없습니다.");
        }
    }
}



// 객체 풀 클래스
public class MonsterPool
{
    private GameObject prefab;
    private Queue<GameObject> pool;

    public MonsterPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        pool = new Queue<GameObject>();

        // 초기 크기만큼 오브젝트를 풀에 미리 추가
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // 풀에서 오브젝트를 가져옴
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue(); 
        }
        else
        {
            // 풀에 객체가 없으면 새로 생성하여 반환
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            return obj;
        }
    }

    // 오브젝트를 풀에 반환
    public void ReturnObject(GameObject obj)
    {
        pool.Enqueue(obj); 
    }
}