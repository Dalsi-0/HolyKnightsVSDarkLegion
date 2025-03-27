using UnityEngine;

namespace Monster
{
    // 임시 데이터입니다.
    public class MonsterData : ScriptableObject
    {
        public string Id;
        public string Name;
        public float MoveSpeed;
        public int MaxHp;
        public float Atk;
        public int AtkRange;
        public float AtkDelay;
    }

    public enum AttackType
    {
        MELEE,
        RANGE,
        SPECIAL,
    }
}