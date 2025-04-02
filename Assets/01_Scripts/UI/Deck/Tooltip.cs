using UnityEngine;
using TMPro;
public class Tooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unitName;
    [SerializeField] TextMeshProUGUI unitAttack;
    [SerializeField] TextMeshProUGUI unitHeath;
    public void Setup(UnitSO unit)
    {
        unitName.text = unit.UnitName;
        unitAttack.text = unit.UnitAtk.ToString();
        unitHeath.text = unit.UnitHP.ToString();
    }
}
