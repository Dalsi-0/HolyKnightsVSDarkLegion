using UnityEngine;

namespace Monsters
{
    public class MonsterBaseState : IState
    {
        protected readonly Monster monster;
        protected readonly MonsterStateMachine stateMachine;
        protected float speedModifier = 1f;

        protected MonsterBaseState(MonsterStateMachine stateMachine)
        {
            monster = stateMachine.Monster;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }

        public virtual void Update()
        {
            Move();
        }

        public virtual void Exit() { }

        private void Move()
        {
            var moveSpeed = monster.MonsterData.MonsterMoveSpeed * speedModifier;
            monster.transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
        }

        protected void StartAnimation(int animHash)
        {
            monster.Animator.SetBool(animHash, true);
        }

        protected void StopAnimation(int animHash)
        {
            monster.Animator.SetBool(animHash, false);
        }

        protected float GetNormalizedTime(string animationName)
        {
            var currentAnimState = monster.Animator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimState.IsName(animationName))
                return currentAnimState.normalizedTime % 1;
            else
                return 0;
        }
    }
}