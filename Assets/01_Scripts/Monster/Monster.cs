using UnityEngine;

namespace Monster
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private MonsterStateMachine stateMachine;
        [SerializeField] private Animator animator;
        
        public MonsterStateMachine StateMachine => stateMachine;
        public Animator Animator => animator;
    }
}