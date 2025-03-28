using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterGridSensor : MonoBehaviour
    {
        public Test gridTest; // 임시
        public MonsterStateMachine StateMachine;

        public Transform Target { get; private set; } = null; // TODO: Unit 클래스가 생기면 변경
        public Vector2Int CurrentCell { get; private set; }
        public Vector2Int FrontCell { get; private set; }
        public WaitForSeconds CheckDelay = new(0.02f);
        public bool IsArrived { get; private set; }

        public Action OnTargetDead;
        
        public void Start()
        {
            StartCoroutine(CheckFrontCell());
        }
        
        private IEnumerator CheckFrontCell()
        {
            while (true)
            {
                CurrentCell = gridTest.WorldToGridCell(StateMachine.Tr.position);
                FrontCell = CurrentCell + Vector2Int.left;
                
                if (gridTest.IsUnitAt(FrontCell))
                {
                    Target = gridTest.Unit;
                    StartCoroutine(CheckArrived());
                    yield break;
                }
                
                yield return CheckDelay;
            }
        }
        
        private IEnumerator CheckArrived()
        {
            while (true)
            {
                if (StateMachine.Tr.position.x - gridTest.GetCellCenter(CurrentCell).x < 0.02f)
                {
                    IsArrived = true;
                    yield break;
                }

                yield return null;
            }
        }
    }
}
