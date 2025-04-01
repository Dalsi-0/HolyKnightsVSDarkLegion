using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    public float speed = 10f;
    public float lifetime = 3f;  // 투사체 수명
    private float damage;   // 투사체 데미지
    private float timer;
    private bool isInitialized = false;
    private ProjectilePoolManager poolManager;

    [SerializeField] private Monsters.Monster monster;

    public void Initialize(Vector2 direction, float damage)
    {
        this.direction = direction;
        this.damage = damage;
        timer = lifetime;
        isInitialized = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // 풀링 매니저 설정 메서드
    public void SetPoolManager(ProjectilePoolManager manager)
    {
        poolManager = manager;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 투사체 이동
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // 수명 관리
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            DeactivateProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized) return;

        // 에너미 레이어를 가진 오브젝트와 충돌했을 때
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 몬스터 컴포넌트 찾기
            Monsters.Monster monster = other.GetComponent<Monsters.Monster>();
            if (monster != null)
            {
                monster.StateMachine.OnHit((int)damage);
            }

            // 충돌 후 투사체 비활성화 및 풀로 반환
            DeactivateProjectile();
        }
    }

    // 투사체 비활성화 및 풀로 반환
    private void DeactivateProjectile()
    {
        isInitialized = false;

        if (poolManager != null)
        {
            poolManager.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 오브젝트가 비활성화될 때 호출 (풀로 반환될 때)
    private void OnDisable()
    {
        isInitialized = false;
    }
}