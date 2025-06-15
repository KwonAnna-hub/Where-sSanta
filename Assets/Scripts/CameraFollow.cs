using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // 따라갈 캐릭터

    [Header("Camera Bounds")]
    public float leftLimit = 0f; // 카메라의 왼쪽 한계
    public float rightLimit = 10f; // 카메라의 오른쪽 한계
    public float bottomLimit = 0;
    public float playerY;

    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f; // 카메라 이동 부드럽게 처리

    void LateUpdate()
    {
        if (target == null || target.GetComponent<PlayerCtrl>().isDead) return;

        // 타겟 위치를 따라 이동
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + playerY, transform.position.z);

        // 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 카메라의 X 좌표를 제한
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, leftLimit, rightLimit);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, bottomLimit, float.MaxValue);

        // 카메라 위치 업데이트
        transform.position = smoothedPosition;
    }

    // 카메라 경계 시각화 (Scene View에서만 보임)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 경계 표시
        Gizmos.DrawLine(new Vector3(leftLimit, transform.position.y - 5, 0), new Vector3(leftLimit, transform.position.y + 5, 0));
        Gizmos.DrawLine(new Vector3(rightLimit, transform.position.y - 5, 0), new Vector3(rightLimit, transform.position.y + 5, 0));
    }
}
