using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 initialCameraPosition;
    public float startX = 0f; // 카메라의 시작 x좌표
    public float endX = 175f; // 카메라의 끝 x좌표
    public Vector3 mountainStartPosition; //산 시작
    public Vector3 mountainEndPosition; //산 끝

    // Start is called before the first frame update
    void Start()
    {
        mountainStartPosition = transform.position;
        initialCameraPosition = cameraTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float cameraDeltaX = cameraTransform.position.x - initialCameraPosition.x;
        float cameraDeltaY = cameraTransform.position.y - initialCameraPosition.y;
        float cameraX = cameraTransform.position.x;

        float t = Mathf.InverseLerp(startX, endX, cameraX);//0~1범위로 보정

        Vector3 targetPosition = mountainStartPosition + new Vector3(cameraDeltaX, cameraDeltaY,0) + t * mountainEndPosition;

        transform.position = targetPosition;
    }
}
