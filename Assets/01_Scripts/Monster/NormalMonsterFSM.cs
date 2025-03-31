namespace Monsters
{
    public class NormalMonsterFSM : MonsterStateMachine
    {
        public override void Init(Monster monster)
        {
            AttackState = new MonsterAttackState(this);
            base.Init(monster);
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