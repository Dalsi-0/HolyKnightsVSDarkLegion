using UnityEngine;


public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    public float speed = 10f;
    public float lifetime = 3f;  // 투사체 수명
    private float damage;   // 투사체 데미지
    private float timer;
    [SerializeField] private Monsters.Monster monster;

    public void Initialize(Vector2 direction, float damage)
    {
        this.direction = direction;
        this.damage = damage;
        timer = lifetime;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        // 투사체 이동
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // 수명 관리
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적 태그를 가진 오브젝트와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            // 몬스터 컴포넌트 찾기
            Monsters.Monster monster = other.GetComponent<Monsters.Monster>();
            if (monster != null)
            {
                monster.StateMachine.OnHit((int)damage);
            }
            
            // 충돌 후 투사체 파괴
            Destroy(gameObject);
        }
    }
}