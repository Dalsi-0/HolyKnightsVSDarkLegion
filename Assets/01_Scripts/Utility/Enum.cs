using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ATK_TYPE
{
    MELEE,
    RANGED,
    SPECIAL
}

public enum ATTACK_RANGE_TYPE
{
    SINGLE,
    VERTICAL,
    HORIZONTAL
}

public enum DEBUFF_TYPE
{
    NONE,
    SLOW,
}

public enum UnitState
{
    Idle,
    Dead,
    Hurt,
    BasicAttack,
    UsingSkill
}
public enum PoolType
{
    Projectile,
    SkillEffect
}
