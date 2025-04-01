using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeMove : MonoBehaviour
{
    public float moveSpeed = 1f;  
    public float moveDistance = .5f; 

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // 최초 위치 저장
    }

    void Update()
    {
        // Sin 값을 이용한 부드러운 왕복
        float newY = startPos.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
