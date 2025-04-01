public interface IDamageable
{
    void TakeDamage(float damage);
}

public interface IWaveState
{
    void EnterState();
    void UpdateState();
}