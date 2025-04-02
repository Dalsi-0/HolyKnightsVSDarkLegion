using System.Collections.Generic;
using UnityEngine;
public class UnitManager : Singleton<UnitManager>
{
    [Header("UnitPrefabs")]
    public GameObject[] unitPrefabs; // 유닛 프리팹 목록
    private Dictionary<string, GameObject> unitDitionary; // 유닛 이름별로 저장
    public GameObject particlePrefab; // 생성시 파티클 프리팹
    private ParticleSystem spawnParticle; // 재사용하기 위해 저장
    public int PlayerMoney; // 소지 자원

    [Header("Map Info")]
    public Vector2 startPos = new Vector2(-11.5f, -7.5f); // 좌하단 기준 좌표
    public Vector2Int tileSize = new Vector2Int(9, 5); // 타일 총 칸
    public Vector2 stepSize = new Vector2(3, 3); // 중심점 간 간격
    private PlayerUnit[,] tileInfo; // 타일위에 소환 가능 여부, false는 이미 유닛 있음, TODO : 유닛 객체 정보 저장 필요
    private UnitCreator unitCreator;
    public UnitCreator UnitCreator
    {
        get
        {
            if (unitCreator == null)
            {
                unitCreator = FindObjectOfType<UnitCreator>();
            }
            return unitCreator;
        }
    }


    protected override void Awake()
    {
        base.Awake();
        tileInfo = new PlayerUnit[tileSize.x, tileSize.y];
        unitDitionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in unitPrefabs)
        {
            unitDitionary[prefab.name] = prefab;
        }
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeMoney(30);
        }
#endif
    }
    // 좌표로 가장 가까운 인덱스 반환
    public Vector2Int GetGridIndex(Vector3 pos)
    {

        // 가장 가까운 중심점 계산

        // 좌표별 차이
        float disX = Mathf.Sqrt((startPos.x - pos.x) * (startPos.x - pos.x));
        float disY = Mathf.Sqrt((startPos.y - pos.y) * (startPos.y - pos.y));

        // 0.5이상은 올리고 미만은 내림, 간격 추가
        int indexX = (int)Mathf.Floor(disX / stepSize.x + 0.5f);
        int indexY = (int)Mathf.Floor(disY / stepSize.y + 0.5f);


        return new Vector2Int(indexX, indexY);
    }

    // 인덱스로 월드 좌표 반환
    public Vector3 GetPosByGrid(int indexX, int indexY)
    {
        Vector3 pos = new Vector3();
        // 범위 오버시 최소, 최대 적용
        indexX = Mathf.Clamp(indexX, 0, tileSize.x - 1);
        indexY = Mathf.Clamp(indexY, 0, tileSize.y - 1);

        pos.x = startPos.x + stepSize.x * indexX;
        pos.y = startPos.y + stepSize.y * indexY;
        return pos;
    }

    public PlayerUnit IsOnUnit(int indexX, int indexY)
    {
        if (indexX < 0 || indexX >= tileSize.x) return null;
        if (indexY < 0 || indexY >= tileSize.y) return null;
        // 소환 가능한 칸인지 판별
        if (tileInfo[indexX, indexY] != null)
        {
            return tileInfo[indexX, indexY];
        }
        return null;
    }


    // 월드 좌표로 생성
    public bool Spawn(string unitID, Vector3 pos)
    {
        // 좌표 범위 확인
        if (IsOnGrid(pos))
        {
            // 그리드 인덱스 알고 생성
            Vector2Int grid = GetGridIndex(pos);
            return Spawn(unitID, grid.x, grid.y);
        }
        return false;
    }

    // ID와 그리드 x,y로 생성
    public bool Spawn(string unitID, int indexX, int indexY)
    {
        // 그리드 확인
        if (indexX < 0 || indexX >= tileSize.x) return false;
        if (indexY < 0 || indexY >= tileSize.y) return false;
        if (IsOnUnit(indexX, indexY)) return false;
        // id 확인
        if (unitDitionary.ContainsKey(unitID))
        {
            // 비용 자동 소모
            if (ChangeMoney(-unitDitionary[unitID].GetComponent<PlayerUnit>().GetUnitData().UnitSummonCost))
            {
                // 월드 좌표로 변환
                Vector3 pos = GetPosByGrid(indexX, indexY);

                // 근처의 중심에 엔티티 소환 
                //PlayerUnit unit = Instantiate(unitPrefab, pos, Quaternion.identity).GetComponent<PlayerUnit>();
                GameObject prefab = unitDitionary[unitID];
                PlayerUnit unit = Instantiate(prefab, pos, Quaternion.identity).GetComponent<PlayerUnit>();
                // 배열에 저장
                if (unit != null)
                {
                    unit.OnPlayerDeadAction += Remove;
                    tileInfo[indexX, indexY] = unit;
                    // 파티클 생성, 재생
                    if (spawnParticle == null)
                    {
                        ParticleSystem particle = Instantiate(particlePrefab, pos, Quaternion.identity).GetComponent<ParticleSystem>();
                        if (particle != null)
                            spawnParticle = particle;
                    }
                    spawnParticle.transform.localPosition = pos;
                    spawnParticle.Play();
                }
                return true;
            }
            else
            {
                //"비용 부족";
            }
        }
        else
        {
           //프리팹 목록에 {unitID} 없음;
        }
        return false;
    }

    // 자원을 추기/소비하는 함수, 소비에 실패하면 falsee 반환
    public bool ChangeMoney(int amount)
    {
        if (amount < 0 && PlayerMoney + amount < 0) return false;
        PlayerMoney += amount;
        // 생성자 UI 갱신
        UnitCreator.ChangeMoney(PlayerMoney);
        return true;
    }
    public bool ChangeMoney(float amount)
    {
        // 반올림 적용
        return ChangeMoney((int)Mathf.Round(amount));
    }
    // 자원을 강제로 바꾸는 함수
    public void SetMoney(int newValue)
    {
        PlayerMoney = newValue;
    }

    // 목록에서 유닛 정보 삭제
    public void Remove(PlayerUnit unit)
    {
        // 유닛 정보 삭제
        Vector3 pos = unit.transform.position;
        Vector2Int grid = GetGridIndex(pos);
        tileInfo[grid.x, grid.y] = null;
    }
    public bool IsOnGrid(Vector3 pos)
    {
        // 좌표 범위가 유효한지 검사
        if (pos.x < startPos.x - stepSize.x / 2)
        {
            // x 범위 부족
            return false;
        }
        else if (pos.x > startPos.x + stepSize.x * (tileSize.x - 1) + stepSize.x / 2)
        {
            // x 범위 초과
            return false;
        }
        else if (pos.y > startPos.y + stepSize.y * (tileSize.y - 1) + stepSize.y / 2)
        {
            // y 범위 초과
            return false;
        }
        else if (pos.y < startPos.y - stepSize.y / 2)
        {
            // y 범위 부족
            return false;
        }
        return true;
    }
    public void SetActiveCreator(bool active)
    {
        UnitCreator.SetActive(active);
    }
}
