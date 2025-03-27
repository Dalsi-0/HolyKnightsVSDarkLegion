using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/Monster Data", order = int.MaxValue)]
public class MonsterSO : ScriptableObject
{
    [SerializeField] private string monsterID;
    [SerializeField] private string monsterName;
    [SerializeField] private float monsterHP;
    [SerializeField] private float monsterAtk;
    [SerializeField] private float monsterAtkRange;
    [SerializeField] private float monsterAtkDelay;
    [SerializeField] private float monsterMoveSpeed;
    [SerializeField] private ATK_TYPE monsterAtkType;

    // Getter
    public string MonsterID => monsterID;
    public string MonsterName => monsterName;
    public float MonsterHP => monsterHP;
    public float MonsterAtk => monsterAtk;
    public float MonsterAtkRange => monsterAtkRange;
    public float MonsterAtkDelay => monsterAtkDelay;
    public float MonsterMoveSpeed => monsterMoveSpeed;
    public ATK_TYPE MonsterAttackType => monsterAtkType;

    public void SetData(string id, string name, float hp, float atk, float atkRange, float atkDelay, float moveSpeed, ATK_TYPE atkType)
    {
        monsterID = id;
        monsterName = name;
        monsterHP = hp;
        monsterAtk = atk;
        monsterAtkRange = atkRange;
        monsterAtkDelay = atkDelay;
        monsterMoveSpeed = moveSpeed;
        monsterAtkType = atkType;
    }
}