using System.Collections;
using UnityEngine;

namespace Monsters
{
    public class MonsterStateMachine : StateMachine
    {
        public Monster Monster { get; private set; }
        
        protected Color baseColor = Color.white;
        protected readonly Color hitColor = new(1f, 0.4f, 0.4f);
        protected Coroutine hitCoroutine;

        public float SpeedModifier { get; private set; } = 1f;
        public float DebuffSpeedModifier { get; private set; } = 1f;
        public float AttackSpeedModifier { get; private set; } = 1f;

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
                if (lastAttackTime < Monster.MonsterData.MonsterAtkDelay * AttackSpeedModifier)
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
            Debug.Log("Hit" + baseColor);
            yield return LerpColor(baseColor, hitColor, hitDuration);
            yield return LerpColor(hitColor, baseColor, hitDuration);

            Monster.SpriteRenderer.color = baseColor;
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
        
        public void SetSpeedModifier(float modifier)
        {
            SpeedModifier = modifier;
        }
        
        public void SetDebuffSpeedModifier(float modifier)
        {
            DebuffSpeedModifier = modifier;
        }
        
        public void SetAttackSpeedModifier(float modifier)
        {
            AttackSpeedModifier = modifier;
        }

        public void SetBaseColor(Color setColor)
        {
            baseColor = setColor;
            Monster.SpriteRenderer.color = baseColor;
        }

        public void OnDead()
        {
            MonsterFactory monsterFactory = StageManager.Instance.GetMonsterFactory();
            monsterFactory.ReturnMonsterToPool(gameObject);
            StageManager.Instance.GetWaveSpawningState().OnMonsterDied();
        }
    }
}