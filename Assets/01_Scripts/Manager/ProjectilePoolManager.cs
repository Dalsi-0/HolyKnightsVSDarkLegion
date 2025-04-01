using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : Singleton<ProjectilePoolManager>
{
    // 프리팹별 오브젝트 풀 관리
    private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();

    // 풀의 부모 Transform (계층구조 정리용)
    private Transform _poolParent;
    private Transform _projectilePoolsParent;
    private Transform _skillEffectPoolsParent;

    // 각 프리팹별 초기 생성 개수
    [System.Serializable]
    public class PoolInfo
    {
        public GameObject prefab;
        public int initialPoolSize = 10;
        public PoolType poolType = PoolType.Projectile;
    }

    public enum PoolType
    {
        Projectile,
        SkillEffect
    }

    // 인스펙터에서 설정 가능한 초기 풀 정보
    public List<PoolInfo> initialPools = new List<PoolInfo>();

    protected override void Awake()
    {
        // Singleton 기본 Awake 호출 (Instance 설정)
        base.Awake();

        // 풀의 부모 오브젝트 생성
        _poolParent = new GameObject("ObjectPools").transform;
        _poolParent.SetParent(transform);

        // 카테고리별 풀 부모 생성
        _projectilePoolsParent = new GameObject("ProjectilePools").transform;
        _projectilePoolsParent.SetParent(_poolParent);

        _skillEffectPoolsParent = new GameObject("SkillEffectPools").transform;
        _skillEffectPoolsParent.SetParent(_poolParent);

        // 초기 풀 생성
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (PoolInfo poolInfo in initialPools)
        {
            CreatePool(poolInfo.prefab, poolInfo.initialPoolSize, poolInfo.poolType);
        }
    }

    // 새 오브젝트 풀 생성
    public void CreatePool(GameObject prefab, int initialSize, PoolType poolType = PoolType.Projectile)
    {
        string prefabName = prefab.name;

        if (!_poolDictionary.ContainsKey(prefabName))
        {
            _poolDictionary[prefabName] = new Queue<GameObject>();
            _prefabDictionary[prefabName] = prefab;

            // 풀의 컨테이너 생성 - 타입에 따라 부모 설정
            Transform parentTransform = (poolType == PoolType.Projectile) ? _projectilePoolsParent : _skillEffectPoolsParent;
            GameObject poolContainer = new GameObject(prefabName + "Pool");
            poolContainer.transform.SetParent(parentTransform);

            // 초기 오브젝트 생성
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = CreateNewInstance(prefab, poolContainer.transform, poolType);
                ReturnToPool(obj);
            }
        }
    }

    // 풀에서 오브젝트 가져오기
    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.Projectile)
    {
        string prefabName = prefab.name;

        // 요청된 프리팹의 풀이 없으면 생성
        if (!_poolDictionary.ContainsKey(prefabName))
        {
            CreatePool(prefab, 10, poolType);
        }

        // 적절한 부모 Transform 가져오기
        Transform parentTransform = (poolType == PoolType.Projectile) ? _projectilePoolsParent : _skillEffectPoolsParent;

        // 풀에 오브젝트가 없으면 새로 생성
        if (_poolDictionary[prefabName].Count == 0)
        {
            GameObject newObj = CreateNewInstance(prefab, parentTransform.Find(prefabName + "Pool"), poolType);
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);
            return newObj;
        }

        // 풀에서 오브젝트 가져오기
        GameObject obj = _poolDictionary[prefabName].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    // 오브젝트를 풀로 반환
    public void ReturnToPool(GameObject obj)
    {
        string prefabName = obj.name.Replace("(Clone)", "").Trim();

        if (_poolDictionary.ContainsKey(prefabName))
        {
            obj.SetActive(false);
            _poolDictionary[prefabName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Trying to return an object that doesn't have a pool: " + prefabName);
            Destroy(obj);
        }
    }

    // 일정 시간 후 오브젝트를 풀로 반환하는 메서드
    public void ReturnToPoolWithDelay(GameObject obj, float delay)
    {
        StartCoroutine(ReturnToPoolAfterDelay(obj, delay));
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null && obj.activeInHierarchy)
        {
            ReturnToPool(obj);
        }
    }

    // 새 인스턴스 생성 메서드
    private GameObject CreateNewInstance(GameObject prefab, Transform parent, PoolType poolType)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.name = prefab.name; // (Clone) 제거

        // 타입에 따라 적절한 초기화 수행
        if (poolType == PoolType.Projectile && obj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.SetPoolManager(this);
        }

        return obj;
    }
}