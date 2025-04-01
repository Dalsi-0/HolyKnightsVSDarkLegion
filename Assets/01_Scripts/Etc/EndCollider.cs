using Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCollider : MonoBehaviour
{
    [SerializeField] private GameObject lifeObj;
    [SerializeField] private GameObject hitObj;
    [SerializeField] private GameObject breakObj;
    private int enemyReached = 0; // 라인에 도달한 적의 수

    private void Start()
    {
        lifeObj.SetActive(true);
        hitObj.SetActive(false);
        breakObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemyReached++;
            hitObj.SetActive(true);

            if (enemyReached >= 2)
            {
                lifeObj.transform.GetComponent<SpriteRenderer>().enabled = false;
                hitObj.SetActive(false);
                breakObj.SetActive(true);
                StageManager.Instance.StageEnd(false);
            }
            collision.GetComponent<Monster>().StateMachine.OnDead();
        }
    }
}
