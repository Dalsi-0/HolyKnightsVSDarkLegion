using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitSO : ScriptableObject
{
    [SerializeField] private string unitID;
    [SerializeField] private string unitName;
    [SerializeField] private float unitHP;
    [SerializeField] private float unitAtk;
    [SerializeField] private float unitAtkRange;
    [SerializeField] private float unitAtkDelay;
    [SerializeField] private float unitSummonCost;
    [SerializeField] private float unitCoolDown;
    [SerializeField] private ATK_TYPE unitAtkType;
    [SerializeField] private Sprite unitSprite;

    // Getter
    public string UnitID => unitID;
    public string UnitName => unitName;
    public float UnitHP => unitHP;
    public float UnitAtk => unitAtk;
    public float UnitAtkRange => unitAtkRange;
    public float UnitAtkDelay => unitAtkDelay;
    public float UnitSummonCost => unitSummonCost;
    public float UnitCoolDown => unitCoolDown;
    public ATK_TYPE UnitAttackType => unitAtkType;
    public Sprite UnitSprite => unitSprite;

    public void SetData(string id, string name, float hp, float atk, float atkRange, float atkDelay, float summonCost, float coolDown, ATK_TYPE atkType)
    {
        unitID = id;
        unitName = name;
        unitHP = hp;
        unitAtk = atk;
        unitAtkRange = atkRange;
        unitAtkDelay = atkDelay;
        unitSummonCost = summonCost;
        unitCoolDown = coolDown;
        unitAtkType = atkType;
    }
}