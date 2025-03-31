using System.Collections;
using UnityEngine;

namespace Monsters
{
    public class MonsterStateMachine : StateMachine
    {
        public Monster Monster { get; private set; }
        
        protected readonly Color origin = Color.white;
        protected readonly Color hit = new(1f, 0.4f, 0.4f);
        protected Coroutine hitCoroutine;
        
        protected const float hitDuration = 0.2f;
        protected float lastAttackTime = 0f;
        protected float currentHp;
        
        #region Properties
        public MonsterIdleState IdleState { get; private set; }
        public MonsterWalkState WalkState { get; private set; }
        public MonsterAttackState AttackState { get; protected set; }
        public MonsterDeadState DeadState { get; private set; }
        #endregion
        
        public virtual void Init(Monster monster)
        {
            // Data
            Monster = monster;
            currentHp = monster.MonsterData.MonsterHP;
            
            // State
            IdleState = new MonsterIdleState(this);
            WalkState = new MonsterWalkState(this);
            DeadState = new MonsterDeadState(this);
            
            ChangeState(WalkState);
            StartCoroutine(CheckAttackTime());
        }
        
        private IEnumerator CheckAttackTime()
        {
            while (true)
            {
                if (lastAttackTime < Monster.MonsterData.MonsterAtkDelay)
                    lastAttackTime += Time.deltaTime;
                
                yield return null;
            }
        }

        public bool CanAttack()
        {
            return lastAttackTime >= Monster.MonsterData.MonsterAtkDelay;
        }

        public virtual void OnAttack() { }

        public void OnHit(int damage)
        {
            currentHp = Mathf.Max(0, currentHp - damage);
            if (currentHp <= 0)
            {
                ChangeState(DeadState);
                return;
            }
            
            hitCoroutine ??= StartCoroutine(HitRoutine());
        }
        
        private IEnumerator HitRoutine()
        {
            yield return LerpColor(origin, hit, hitDuration);
            yield return LerpColor(hit, origin, hitDuration);

            Monster.SpriteRenderer.color = origin;
            hitCoroutine = null;
        }

        private IEnumerator LerpColor(Color from, Color to, float duration)
        {
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                Monster.SpriteRenderer.color = Color.Lerp(from, to, time / duration);
                yield return null;
            }
        }

        public void OnDead()
        {
            
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}