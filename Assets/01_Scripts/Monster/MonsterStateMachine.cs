using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterStateMachine : StateMachine, IEnemyUnit
    {
        [field: SerializeField] public Animator Anim { get; private set; }
        [field: SerializeField] public Transform Tr { get; private set; }

        // 임시
        public string Id { get; }
        public string Name { get; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public float AtkRange { get; set; } = 1f;
        public float AtkDelay { get; set; } = 2f;
        public float MoveSpeed { get; set; } = 1.5f;
        public float MoveSpeedModifier { get; set; } = 1f;
        //

        private IdleState idleState;
        private WalkState walkState;

        public void Start()
        {
            // 초기화
            idleState = new IdleState(this);
            walkState = new WalkState(this);
            
            ChangeState(walkState);
        }
    }
}