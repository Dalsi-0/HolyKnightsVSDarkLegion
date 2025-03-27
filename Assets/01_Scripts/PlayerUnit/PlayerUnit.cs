using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour, IGameEntity, IAttacker, IPlayerUnit
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int Hp { get; set; }
    public int Atk { get; set; }
    public float AtkRange { get; set; }
    public float AtkDelay { get; set; }
    public int SummonCost { get; set; }
    public int CoolDown { get; set; }

    public PlayerUnit(string id, string name, int hp, int atk, float atkRange, float atkDelay, int summonCost, int coolDown)
    {
        Id = id;
        Name = name;
        Hp = hp;
        Atk = atk;
        AtkRange = atkRange;
        AtkDelay = atkDelay;
        SummonCost = summonCost;
        CoolDown = coolDown;
    }

    public void Attack()
    {
        // 공격 로직을 구현합니다
    }

    public void UseSkill()
    {
        // 스킬 사용 로직을 구현합니다
    }
}
