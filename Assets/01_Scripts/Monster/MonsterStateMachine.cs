using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterStateMachine : StateMachine, IEnemyUnit
    {
        [field: SerializeField] public Transform Tr { get; private set; }
        [field: SerializeField] public Animator Anim { get; private set; }
        [field: SerializeField] public SpriteRenderer Renderer { get; private set; }
        [field: SerializeField] public MonsterGridSensor GridSensor { get; private set; }

        // 임시
        public string Id { get; }
        public string Name { get; }
        public int Hp { get; set; } = 100;
        public int Atk { get; set; } = 10;
        public float AtkRange { get; set; } = 1f;
        public float AtkDelay { get; set; } = 2f;
        public float MoveSpeed { get; set; } = 1.5f;
        public float MoveSpeedModifier { get; set; } = 1f;
        //

        public MonsterIdleState IdleState { get; private set; }
        public MonsterWalkState WalkState { get; private set; }
        public MonsterAttackState AttackState { get; private set; }
        public MonsterDeadState DeadState { get; private set; }
        private float lastAttackTime;
        
        private readonly Color origin = Color.white;
        private readonly Color hit = new(1f, 0.4f, 0.4f);
        private Coroutine hitCoroutine;
        private const float hitTime = 0.2f;

        public void Start()
        {
            // 초기화
            GridSensor.Init(this);
            IdleState = new MonsterIdleState(this);
            WalkState = new MonsterWalkState(this);
            AttackState = new MonsterAttackState(this);
            DeadState = new MonsterDeadState(this);
            
            ChangeState(WalkState);
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

        public void OnAttack()
        {
            lastAttackTime = 0;
            // TODO: Target.OnHit(Atk);
        }

        public void OnHit(int damage)
        {
            Hp = Mathf.Max(0, Hp - damage);
            if (Hp <= 0)
            {
                ChangeState(DeadState);
                return;
            }
            
            hitCoroutine ??= StartCoroutine(HitRoutine());
        }
        
        private IEnumerator HitRoutine()
        {
            yield return LerpColor(origin, hit, hitTime);
            yield return LerpColor(hit, origin, hitTime);

            Renderer.color = origin;
            hitCoroutine = null;
        }

        private IEnumerator LerpColor(Color from, Color to, float duration)
        {
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                Renderer.color = Color.Lerp(from, to, time / duration);
                yield return null;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}