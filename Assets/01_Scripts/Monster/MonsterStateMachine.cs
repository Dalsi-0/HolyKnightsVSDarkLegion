using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterStateMachine : StateMachine, IEnemyUnit
    {
        [field: SerializeField] public Transform Tr { get; private set; }
        [field: SerializeField] public Animator Anim { get; private set; }
        [field: SerializeField] public MonsterGridSensor GridSensor { get; private set; }

        // 임시
        public string Id { get; }
        public string Name { get; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public float AtkRange { get; set; } = 1f;
        public float AtkDelay { get; set; } = 2f;
        public float MoveSpeed { get; set; } = 1.5f;
        public float MoveSpeedModifier { get; set; } = 1f;

        public Test gridTest;
        //

        public Vector3 Destination { get; private set; } = Vector3.zero;
        public float lastAttackTime { get; set; }
        public MonsterIdleState idleState { get; private set; }
        public MonsterWalkState walkState { get; private set; }
        public MonsterAttackState attackState { get; private set; }

        public void Start()
        {
            Destination = gridTest.GetCellCenter(new Vector2Int(-2, 2));
            
            // 초기화
            idleState = new MonsterIdleState(this);
            walkState = new MonsterWalkState(this);
            attackState = new MonsterAttackState(this);
            
            ChangeState(walkState);
            StartCoroutine(CheckAttackTime());
        }
        
        private IEnumerator CheckAttackTime()
        {
            while (true)
            {
                if (lastAttackTime < AtkDelay)
                    lastAttackTime += Time.deltaTime;
                
                yield return null;
            }
        }

        public bool CanAttack()
        {
            return lastAttackTime >= AtkDelay;
        }
    }
}