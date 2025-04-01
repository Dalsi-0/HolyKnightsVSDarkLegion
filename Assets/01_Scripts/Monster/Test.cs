using System;
using System.Collections.Generic;
using Monsters;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> list;
    [SerializeField] private Monster monster;

    private bool onGUI = false;
    
    private void Start()
    {
        UnitManager.Instance.ChangeMoney(9999);
        
        for (int i = 0; i < list.Count; i++)
        {
            var index = list[i];
            UnitManager.Instance.Spawn("PK_002", index.x, index.y);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            onGUI = !onGUI;
        }
    }

    private void OnGUI()
    {
        if (!onGUI) return;
        
        var slow = new SlowDebuff(2f, 0.5f, new Color(0.35f, 0.82f, 1f));
        
        GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button);
        bigButtonStyle.fontSize = 35;
        bigButtonStyle.fixedHeight = 80;
        bigButtonStyle.fixedWidth = 150; 
        if (GUILayout.Button("Slow", bigButtonStyle))
        {
            monster.DebuffHandler.ExecuteDebuff(slow);
        }
        
        if (GUILayout.Button("Hit", bigButtonStyle))
        {
            monster.StateMachine.OnHit(20);
        }
    }
}
