namespace Monsters
{
    public class WereWolfStateMachine : MonsterStateMachine
    {
        public override void Init(Monster monster)
        {
            AttackState = new WereWolfAttackState(this);
            base.Init(monster);
        }
        
        public override void OnAttack()
        {
            var attackState = (WereWolfAttackState)AttackState;
            if (attackState.IsFirstAttack)
            {
                // frontCell의 Center로 이동
                Monster.GridSensor.SetTarget();
                var destination = Monster.GridSensor.GetFrontCellCenter();
            }
        }
    }
}