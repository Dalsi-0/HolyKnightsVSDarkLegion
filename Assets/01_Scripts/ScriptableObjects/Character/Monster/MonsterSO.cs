using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/Monster Data", order = int.MaxValue)]
public class MonsterSO : ScriptableObject
{
    [SerializeField] private string monsterID;
    [SerializeField] private string monsterName;
    [SerializeField] private float monsterHP;
    [SerializeField] private float monsterAtk;
    [SerializeField] private int monsterAtkRange;
    [SerializeField] private float monsterAtkDelay;
    [SerializeField] private ATTACK_RANGE_TYPE monsterAttackRangeType;
    [SerializeField] private float monsterMoveSpeed;
    [SerializeField] private ATK_TYPE monsterAtkType;

    // Getter
    public string MonsterID => monsterID;
    public string MonsterName => monsterName;
    public float MonsterHP => monsterHP;
    public float MonsterAtk => monsterAtk;
    public int MonsterAtkRange => monsterAtkRange;
    public float MonsterAtkDelay => monsterAtkDelay;
    public ATTACK_RANGE_TYPE MonsterAttackRangeType => monsterAttackRangeType;
    public float MonsterMoveSpeed => monsterMoveSpeed;
    public ATK_TYPE MonsterAttackType => monsterAtkType;

    public void SetData(string id, string name, float hp, float atk, int atkRange, float atkDelay, ATTACK_RANGE_TYPE attackRangeType, float moveSpeed, ATK_TYPE atkType)
    {
        monsterID = id;
        monsterName = name;
        monsterHP = hp;
        monsterAtk = atk;
        monsterAtkRange = atkRange;
        monsterAtkDelay = atkDelay;
        monsterAttackRangeType = attackRangeType;
        monsterMoveSpeed = moveSpeed;
        monsterAtkType = atkType;
    }
}