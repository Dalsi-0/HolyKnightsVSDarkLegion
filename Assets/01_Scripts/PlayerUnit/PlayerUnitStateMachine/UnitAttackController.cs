using UnityEngine;

public class UnitAttackController : MonoBehaviour
{
    private PlayerUnit _playerUnit;
    private UnitSO _unitData;
    private GameObject _projectilePrefab;
    private Transform _firePoint;
    private LayerMask _enemyLayer;
    private float _attackCooldown = 0f;
    private Vector3 _targetPosition;

    public void Initialize(PlayerUnit playerUnit, UnitSO unitData, GameObject projectilePrefab, Transform firePoint, LayerMask enemyLayer)
    {
        _playerUnit = playerUnit;
        _unitData = unitData;
        _projectilePrefab = projectilePrefab;
        _firePoint = firePoint;
        _enemyLayer = enemyLayer;
    }

    public void UpdateAttackCooldown(float deltaTime)
    {
        if (_attackCooldown > 0)
            _attackCooldown -= deltaTime;
    }

    public bool CanAttack()
    {
        return _attackCooldown <= 0;
    }

    public void DetectAndAttackEnemy()
    {
        // 가장 가까운 적 찾기
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, _unitData.UnitAtkRange, _enemyLayer);
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        // 적이 감지되면 공격 상태 설정
        if (closestEnemy != null)
        {
            _playerUnit.SetCurrentTarget(closestEnemy);
            _targetPosition = closestEnemy.position;

            if (CanAttack())
            {
                _playerUnit.GetAnimationController().SetAttackAnimation(true);
                _attackCooldown = _unitData.UnitAtkDelay; // 공격 딜레이 설정
            }
        }
        else
        {
            _playerUnit.SetCurrentTarget(null);
            _playerUnit.GetAnimationController().SetAttackAnimation(false);
        }
    }

    public void FireProjectile()
    {
        Transform currentTarget = _playerUnit.GetCurrentTarget();
        if (_projectilePrefab != null && currentTarget != null)
        {
            // 발사 위치 설정
            Vector3 spawnPosition = _firePoint != null ? _firePoint.position : transform.position;

            // 타겟 방향으로 발사 방향 계산
            Vector2 direction = ((Vector2)(currentTarget.position - spawnPosition)).normalized;

            // 투사체 생성 및 초기화
            GameObject projectileObj = Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity);
            if (projectileObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(direction, _unitData.UnitAtk); // 유닛 공격력 전달
            }
        }
    }
}