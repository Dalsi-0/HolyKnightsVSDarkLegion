using Monsters;
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

    // 직선 공격 설정
    [SerializeField] private bool _attackInRow = true; // true: 가로(행) 공격, false: 세로(열) 공격

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
        // 현재 유닛의 그리드 위치 가져오기
        Vector2Int currentGrid = UnitManager.Instance.GetGridIndex(transform.position);
        // UnitAtkRange를 그리드 거리로 해석
        int gridRange = Mathf.RoundToInt(_unitData.UnitAtkRange);
        // 먼저 넓은 원으로 적들을 검색 (최적화를 위해)
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            gridRange * Mathf.Max(UnitManager.Instance.stepSize.x, UnitManager.Instance.stepSize.y) * 2, // 직선이므로 더 넓게 검사
            _enemyLayer
        );
        // 같은 행/열에 있는 가장 가까운 적 찾기
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;
        foreach (var enemy in enemies)
        {
            // 적의 그리드 위치 계산
            Vector2Int enemyGrid = UnitManager.Instance.GetGridIndex(enemy.transform.position);
            bool isInSameLine = false;
            bool isInFrontDirection = false;

            if (_attackInRow)
            {
                // 같은 행(y값이 같음)에 있고 공격 범위 내인지 확인
                isInSameLine = (enemyGrid.y == currentGrid.y) &&
                              (Mathf.Abs(enemyGrid.x - currentGrid.x) <= gridRange);

                // 오른쪽 방향에 있는지 확인 (왼쪽/뒤쪽은 공격하지 않음)
                isInFrontDirection = enemyGrid.x > currentGrid.x;
            }
            else
            {
                // 같은 열(x값이 같음)에 있고 공격 범위 내인지 확인  
                isInSameLine = (enemyGrid.x == currentGrid.x) &&
                              (Mathf.Abs(enemyGrid.y - currentGrid.y) <= gridRange);

                // 위쪽 방향에 있는지 확인 (아래쪽/뒤쪽은 공격하지 않음)
                // 참고: Unity의 2D 좌표계에서 y가 더 클수록 위
                isInFrontDirection = enemyGrid.y > currentGrid.y;
            }

            // 직선 범위 내에 있고, 앞쪽 방향에 있으며, 더 가까운 적인 경우 업데이트
            if (isInSameLine && isInFrontDirection)
            {
                // 실제 거리 계산 (우선순위 결정용)
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
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
                _attackCooldown = _unitData.UnitAtkDelay; // 공격 딜레이 설정 (별표 제거함)
            }
        }
        else
        {
            _playerUnit.SetCurrentTarget(null);
            _playerUnit.GetAnimationController().SetAttackAnimation(false);
        }
    }
    public void Attack()
    {
        Transform currentTarget = _playerUnit.GetCurrentTarget();
        if (currentTarget != null)
        {
            // 현재 타겟에서 Monster 컴포넌트 찾기
            Monster monster = currentTarget.GetComponent<Monster>();
            if (monster != null)
            {
                // 유닛의 공격력 데이터 사용
                int damage = (int)_unitData.UnitAtk;

                // 몬스터에게 데미지 전달
                monster.StateMachine.OnHit(damage);

                // 공격 성공 후 쿨다운 갱신
                _attackCooldown = _unitData.UnitAtkDelay;
            }
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

    // 기즈모로 직선 공격 범위 시각화
    private void OnDrawGizmos()
    {
        if (_unitData == null || UnitManager.Instance == null) return;

        Gizmos.color = Color.yellow;

        // 현재 유닛의 그리드 위치 가져오기
        Vector2Int currentGrid = UnitManager.Instance.GetGridIndex(transform.position);

        // UnitAtkRange를 그리드 거리로 해석
        int gridRange = Mathf.RoundToInt(_unitData.UnitAtkRange);

        if (_attackInRow) // 가로(행) 공격
        {
            for (int x = -gridRange; x <= gridRange; x++)
            {
                // 현재 행에서 x만 변경
                int targetX = currentGrid.x + x;
                int targetY = currentGrid.y;

                // 유효한 그리드 위치인지 확인
                if (targetX >= 0 && targetX < UnitManager.Instance.tileSize.x &&
                    targetY >= 0 && targetY < UnitManager.Instance.tileSize.y)
                {
                    // 그리드 중심 위치 가져오기
                    Vector3 targetPos = UnitManager.Instance.GetPosByGrid(targetX, targetY);

                    // 그리드 크기에 맞게 사각형 그리기
                    Gizmos.DrawWireCube(targetPos, new Vector3(
                        UnitManager.Instance.stepSize.x * 0.8f,
                        UnitManager.Instance.stepSize.y * 0.8f,
                        0.1f));
                }
            }
        }
        else // 세로(열) 공격
        {
            for (int y = -gridRange; y <= gridRange; y++)
            {
                // 현재 열에서 y만 변경
                int targetX = currentGrid.x;
                int targetY = currentGrid.y + y;

                // 유효한 그리드 위치인지 확인
                if (targetX >= 0 && targetX < UnitManager.Instance.tileSize.x &&
                    targetY >= 0 && targetY < UnitManager.Instance.tileSize.y)
                {
                    // 그리드 중심 위치 가져오기
                    Vector3 targetPos = UnitManager.Instance.GetPosByGrid(targetX, targetY);

                    // 그리드 크기에 맞게 사각형 그리기
                    Gizmos.DrawWireCube(targetPos, new Vector3(
                        UnitManager.Instance.stepSize.x * 0.8f,
                        UnitManager.Instance.stepSize.y * 0.8f,
                        0.1f));
                }
            }
        }
    }

    // 공격 방향 설정 메서드 (필요시 외부에서 변경 가능)
    public void SetAttackDirection(bool attackInRow)
    {
        _attackInRow = attackInRow;
    }
}