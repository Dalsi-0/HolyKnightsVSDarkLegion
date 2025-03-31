using UnityEngine;

// PlayerUnit 클래스에 필요한 메서드가 없는 경우를 대비한 확장 메서드
public static class PlayerUnitExtensions
{
    // enemyLayer 접근을 위한 확장 메서드
    // PlayerUnit에 이미 이 메서드가 있다면 삭제하셔도 됩니다.
    public static LayerMask GetEnemyLayer(this PlayerUnit playerUnit)
    {
        // 리플렉션을 사용해 enemyLayer 필드 가져오기
        System.Reflection.FieldInfo enemyLayerField =
            typeof(PlayerUnit).GetField("enemyLayer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (enemyLayerField != null)
        {
            return (LayerMask)enemyLayerField.GetValue(playerUnit);
        }

        // 기본값 반환 (모든 레이어)
        return -1;
    }

    // UnitData 접근을 위한 확장 메서드
    // PlayerUnit에 이미 GetUnitData() 메서드가 있다면 삭제하셔도 됩니다.
    public static UnitSO GetUnitData(this PlayerUnit playerUnit)
    {
        // 리플렉션을 사용해 unitData 필드 가져오기
        System.Reflection.FieldInfo unitDataField =
            typeof(PlayerUnit).GetField("unitData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (unitDataField != null)
        {
            return (UnitSO)unitDataField.GetValue(playerUnit);
        }

        // 차선책: public 속성이 있는지 확인
        System.Reflection.PropertyInfo unitDataProperty =
            typeof(PlayerUnit).GetProperty("UnitData");

        if (unitDataProperty != null)
        {
            return (UnitSO)unitDataProperty.GetValue(playerUnit);
        }

        return null;
    }
}