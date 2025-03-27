using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private Dictionary<string, UnitSO> UnitSODatas;
    private Dictionary<string, MonsterSO> MonsterSODatas;

    private void SetUnitDatas()
    {

    }

    private void SetMonsterDatas()
    {

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
}
