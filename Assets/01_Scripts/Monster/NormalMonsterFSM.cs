namespace Monsters
{
    // 노말 공격 몬스터 FSM
    public class NormalMonsterFSM : MonsterStateMachine
    {
        public override void Init(Monster monster)
        {
            base.Init(monster);
            AttackState = new MonsterAttackState(this);
        }

        public override void OnAttack()
        {
            var targets = Monster.GridSensor.Targets;
            lastAttackTime = 0;
            for (int i = 0; i < targets.Length; i++)
            {
                if (!targets[i]) continue;
                targets[i].TakeDamage(Monster.MonsterData.MonsterAtk);
            }
        }
    }
}