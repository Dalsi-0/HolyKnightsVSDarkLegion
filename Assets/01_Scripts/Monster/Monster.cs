
using UnityEngine;
namespace Monsters
{
    public class Monster : MonoBehaviour
    {
        [Header("Monster Info")]
        [SerializeField] private string monsterId;
        
        [Header("Components")]
        [SerializeField] private MonsterStateMachine stateMachine;
        [SerializeField] private MonsterGridSensor gridSensor;
        [SerializeField] private MonsterDebuffHandler debuffHandler;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        public string MonsterId => monsterId;
        public MonsterStateMachine StateMachine => stateMachine;
        public MonsterGridSensor GridSensor => gridSensor;
        public MonsterDebuffHandler DebuffHandler => debuffHandler;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Animator Animator => animator;
        public MonsterSO MonsterData { get; private set; }

        private void Start()
        {
            MonsterData = DataManager.Instance.GetMonsterData(monsterId);
            stateMachine.Init(this);
            gridSensor.Init(MonsterData);
            debuffHandler.Init(this);
        }
    }
}