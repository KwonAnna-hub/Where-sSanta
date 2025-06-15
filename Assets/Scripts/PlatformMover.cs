using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float moveSpeed = 2f; // 이동 속도
    public float moveRange = 3f; // 이동 범위

    private float startingPositionX; // 시작 위치
    private int direction = 1; // 이동 방향 (1: 오른쪽, -1: 왼쪽)

    void Start()
    {
        // 시작 위치를 저장
        startingPositionX = transform.position.x;
    }

    void Update()
    {
        // 현재 위치를 업데이트
        float newPositionX = transform.position.x + direction * moveSpeed * Time.deltaTime;

        // 이동 범위를 벗어나면 방향을 전환
        if (Mathf.Abs(newPositionX - startingPositionX) > moveRange)
        {
            direction *= -1; // 방향 반전
            newPositionX = transform.position.x + direction * moveSpeed * Time.deltaTime; // 위치 조정
        }

        // 새로운 위치를 적용
        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);
    }
}
