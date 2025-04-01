using Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCollider : MonoBehaviour
{
    private int enemyReached = 0; // 라인에 도달한 적의 수

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemyReached++;

            if (enemyReached >= 2)
            {
                GameManager.Instance.GameOver();
            }
            collision.GetComponent<Monster>().StateMachine.OnDead();
        }
    }
}
