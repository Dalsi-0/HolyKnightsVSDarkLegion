public interface IGameEntity
{
    string Id { get; }
    string Name { get; }
    int Hp { get; set; }
}

public interface IAttacker
{
    int Atk { get; set; }
    float AtkRange { get; set; }
    float AtkDelay { get; set; }
}

public interface IMovable
{
    float MoveSpeed { get; set; }
}

public interface IPlayerUnit : IGameEntity, IAttacker
{
    int SummonCost { get; set; }
    int CoolDown { get; set; }
}

public interface IEnemyUnit : IGameEntity, IAttacker, IMovable
{
    // 적 고유 속성을 여기에 추가할 수 있습니다
}

public interface IWaveState
{
    void EnterState();
    void UpdateState();
}