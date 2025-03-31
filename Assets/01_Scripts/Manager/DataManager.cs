using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // Temporary use
    [SerializeField] List<UnitSO> listUnitSODatas;
    [SerializeField] List<MonsterSO> listMonsterSODatas;
    [SerializeField] List<StageSO> listStageSODatas;
    //

    private Dictionary<string, UnitSO> UnitSODatas;
    private Dictionary<string, MonsterSO> MonsterSODatas;
    private Dictionary<int, StageSO> StageSODatas;


    protected override void Awake()
    {
        base.Awake();
        initDic();
    }

    // Temporary use
    private void initDic()
    {
        UnitSODatas = new Dictionary<string, UnitSO>();
        MonsterSODatas = new Dictionary<string, MonsterSO>();
        StageSODatas = new Dictionary<int, StageSO>();

        foreach (var unitSO in listUnitSODatas)
        {
            if (unitSO != null && !UnitSODatas.ContainsKey(unitSO.UnitID))
            {
                UnitSODatas.Add(unitSO.UnitID, unitSO);
            }
        }

        foreach (var monsterSO in listMonsterSODatas)
        {
            if (monsterSO != null && !MonsterSODatas.ContainsKey(monsterSO.MonsterID))
            {
                MonsterSODatas.Add(monsterSO.MonsterID, monsterSO);
            }
        }

        foreach (var stageSO in listStageSODatas)
        {
            if (stageSO != null && !StageSODatas.ContainsKey(stageSO.StageNumber))
            {
                StageSODatas.Add(stageSO.StageNumber, stageSO);
            }
        }
    }
    //

    public void SetDatas<T>(List<T> dataList) where T : ScriptableObject
    {
        if (typeof(T) == typeof(UnitSO))
        {
            listUnitSODatas = dataList as List<UnitSO>;
        }
        else if (typeof(T) == typeof(MonsterSO))
        {
            listMonsterSODatas = dataList as List<MonsterSO>;
        }
        else if (typeof(T) == typeof(StageSO))
        {
            listStageSODatas = dataList as List<StageSO>;
        }
    }



    private void LoadUnitDatas()
    {

    }

    private void LoadMonsterDatas()
    {

    }



    public UnitSO GetUnitData(string Id)
    {
        if (UnitSODatas.TryGetValue(Id, out UnitSO unitData))
        {
            return unitData;
        }
        return null;
    }

    public MonsterSO GetMonsterData(string Id)
    {
        if (MonsterSODatas.TryGetValue(Id, out MonsterSO monsterData))
        {
            return monsterData;
        }
        return null;
    }

    public StageSO GetStageData(int stageNumber)
    {
        if (StageSODatas.TryGetValue(stageNumber, out StageSO stageData))
        {
            return stageData;
        }
        return null;
    }


}
