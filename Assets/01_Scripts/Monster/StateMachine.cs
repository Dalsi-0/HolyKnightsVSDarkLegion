using UnityEngine;

namespace Monsters
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }
    
    public class StateMachine : MonoBehaviour
    {
        private IState currentState;

        public void ChangeState(IState state)
        {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
        
        private void Update()
        {
            currentState?.Update();
        }
    }
}