namespace Monsters
{
    // 웨어울프 FSM
    public class WereWolfStateMachine : MonsterStateMachine
    {
        public bool IsFirstAttack { get; private set; } = true;
        
        public override void Init(Monster monster)
        {
            base.Init(monster);
            AttackState = new WereWolfAttackState(this);
        }
        
        public override void OnAttack()
        {
            if (IsFirstAttack)
            {
                SoundManager.Instance.SetSfx(3);
                
                // Front Cell Center에 위치하도록 이동
                var destination = Monster.GridSensor.GetFrontCellCenter();
                Monster.transform.position = destination;
                
                // Current Cell 기준으로 타겟을 설정
                Monster.GridSensor.FindTarget();
                IsFirstAttack = false;
            }
            else
            {
                SoundManager.Instance.SetSfx(2);
            }
            
            // 공격
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