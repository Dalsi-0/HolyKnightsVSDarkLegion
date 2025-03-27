using UnityEngine;

namespace Monster
{
    public class MonsterBaseState : IState
    {
        protected MonsterStateMachine stateMachine;
        
        public MonsterBaseState(MonsterStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        
        public virtual void Enter()
        {
        }
        
        public virtual void Update()
        {
            Move();
        }
        
        public virtual void Exit()
        {
        }

        private void Move()
        {
            var moveSpeed = stateMachine.MoveSpeed * stateMachine.MoveSpeedModifier;
            stateMachine.Tr.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
        }

        protected void StartAnimation(int animHash)
        {
            stateMachine.Anim.SetBool(animHash, true);
        }
        
        protected void StopAnimation(int animHash)
        {
            stateMachine.Anim.SetBool(animHash, false);
        }
    }
}