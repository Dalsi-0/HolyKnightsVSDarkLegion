using System.Collections;
using UnityEngine;

namespace Monster
{
    public class MonsterStateMachine : StateMachine
    {
        [Header("Monster Data")]
        [SerializeField] private string monsterId; 
        
        [field: Header("Components")]
        [field: SerializeField] public Transform Tr { get; private set; }
        [field: SerializeField] public Animator Anim { get; private set; }
        [field: SerializeField] public SpriteRenderer Renderer { get; private set; }
        [field: SerializeField] public MonsterGridSensor GridSensor { get; private set; }

        private readonly Color origin = Color.white;
        private readonly Color hit = new(1f, 0.4f, 0.4f);
        private Coroutine hitCoroutine;
        
        private const float hitTime = 0.2f;
        private float lastAttackTime = 0f;
        private float currentHp;
        
        #region State Properties
        public MonsterSO MonsterData { get; private set; }
        public MonsterIdleState IdleState { get; private set; }
        public MonsterWalkState WalkState { get; private set; }
        public MonsterAttackState AttackState { get; private set; }
        public MonsterDeadState DeadState { get; private set; }
        #endregion
        
        public void Start()
        {
            // Data
            MonsterData = DataManager.Instance.GetMonsterData(monsterId);
            currentHp = MonsterData.MonsterHP;
            
            // Components
            GridSensor.Init(this);
            
            // State
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
                if (lastAttackTime < MonsterData.MonsterAtkDelay)
                    lastAttackTime += Time.deltaTime;
                
                yield return null;
            }
        }

        public bool CanAttack()
        {
            return lastAttackTime >= MonsterData.MonsterAtkDelay;
        }

        public void OnAttack()
        {
            lastAttackTime = 0;
            // TODO: Target.OnHit(Atk);
        }

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