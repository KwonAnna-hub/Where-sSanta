using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoverTB : MonoBehaviour
{
    public float moveSpeed = 2f; // 이동 속도
    public float moveRange = 3f; // 이동 범위

    private float startingPositionY; // 시작 위치
    private int direction = 1; // 이동 방향 (1: 오른쪽, -1: 왼쪽)

    void Start()
    {
        // 시작 위치를 저장
        startingPositionY = transform.position.y;
    }

    void Update()
    {
        // 현재 위치를 업데이트
        float newPositionY = transform.position.y + direction * moveSpeed * Time.deltaTime;

        // 이동 범위를 벗어나면 방향을 전환
        if (Mathf.Abs(newPositionY - startingPositionY) > moveRange)
        {
            direction *= -1; // 방향 반전
            newPositionY = transform.position.y + direction * moveSpeed * Time.deltaTime; // 위치 조정
        }

        // 새로운 위치를 적용
        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
    }
}
